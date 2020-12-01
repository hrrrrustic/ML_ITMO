using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DecisionTrees;

namespace MLCodeForces
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Decision Trees");
            string path = @"D:\RandomTrash\DT\20_train.csv";
            var dataSet = ParseFile(path);
            var tree = DecisionTree.BuildFromDataSet(dataSet, 25);
            var classCount = ParseFile(path.Replace("test", "train")).GroupBy(k => k.Label).Count();
            dataSet = ParseFile(path.Replace("train", "test"));
            var confusion = GetFScores(tree, dataSet, false, classCount, 25);
            Console.WriteLine(confusion.Micro);
            Console.WriteLine(confusion.Macro);
            //GetGraphic(path, 25);
        }

        private static void GetGraphic(DecisionTree tree, DataSetObject[] dataSet, int classCount, int treeHeight)
        {
            List<Double> x = new List<Double>();
            List<Double> y = new List<Double>();
            for (int i = 1; i < 25; i++)
            {
                var fScores = GetFScores(tree, dataSet, false, classCount, treeHeight);
                x.Add(i);
                y.Add((fScores.Micro + fScores.Macro) / 2);
            }

        }
        private static void TestAllFiles(string folderPath)
        {
            Parallel.ForEach(Directory
                .EnumerateFiles(folderPath)
                .Where(k => k.Contains("train")), (s, state) => TestFile(s));
        }

        private static void TestFile(string filePath)
        {
            var dataSet = ParseFile(filePath);
            var classCount = dataSet.GroupBy(k => k.Label).Count();
            var indexFlag = dataSet.Any(k => k.Label == 0);
            var testDataSet = ParseFile(filePath.Replace("train", "test"));

            (double bestMicro, double bestMacro, int bestHeight) = (0, 0, 0);
            var tree = DecisionTree.BuildFromDataSet(dataSet, 25);

            for (int i = 1; i < 25; i++)
            {
                var fScores = GetFScores(tree, testDataSet, indexFlag, classCount, i);

                if (fScores.Macro > bestMacro && fScores.Micro > bestMicro)
                    (bestMacro, bestMicro, bestHeight) = (fScores.Macro, fScores.Micro, i);

                if(bestMacro == 1 && bestMicro == 1)
                    break;
            }

            File.AppendAllText(@"D:\RandomTrash\DTLog.txt", 
                filePath + " : " + bestMacro + " : " + bestMicro + " : " + bestHeight + " : " + Environment.NewLine);
        }
        private static (Double Macro, Double Micro) GetFScores(DecisionTree tree, DataSetObject[] dataSet, bool indexFlag, int classCount, int maxHeight)
        {
            var matrix = new Int32[classCount, classCount];
            foreach (var item in dataSet)
            {
                var predict = tree.ClassifyObject(item, maxHeight);
                var actual = item.Label;
                if (!indexFlag)
                {
                    predict--;
                    actual--;
                }
                matrix[predict, actual]++;
            }

            var confusion = new DecisionTrees.ConfusionMatrix(matrix);
            return (confusion.MacroF1Score(1), confusion.MicroF1Score(1));
        }
        private static DataSetObject[] ParseFile(string path) 
            => File
                .ReadAllLines(path)
                .Skip(1)
                .Select(k => k
                    .Split(',')
                    .Select(Double.Parse)
                    .ToList())
                .Select(k => new DataSetObject(k.SkipLast(1).ToArray(), (int) k[^1]))
                .ToArray();
    }
}