using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bayes;
using XPlot.Plotly;

namespace MLCodeForces
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataFolderPath = @"D:\RandomTrash\Bayes";
            var dataPartPaths = Directory.GetDirectories(dataFolderPath);
            List<List<Message>> dataSet = dataPartPaths
                .Select(k => ParsePart(k, 2))
                .ToList();
            Graphic(dataSet);
            int spamCount = dataSet.Select(k => k.Count(e => e.IsSpam)).Sum();
            int normalCount = dataSet.Select(k => k.Count(e => !e.IsSpam)).Sum();
            var confusionMatrix = CrossValidation.Validate(dataSet, 10, 0.015, 2.5, 1, out var rocData);
            Console.WriteLine(confusionMatrix.Precision(0));
            Console.WriteLine(confusionMatrix.Precision(1));
            rocData = rocData.OrderBy(k => k.Item1).ToList();
            double rightStep = (double) 1 / spamCount;
            double upStep = (double)1 / normalCount;
            List<double> xValues = new List<Double>{0};
            List<double> yValues = new List<Double>{0};
            foreach (var tuple in rocData)
            {
                if (tuple.Item2 == 0)
                {
                    xValues.Add(xValues[^1]);
                    yValues.Add(yValues[^1] + upStep);
                }
                else
                {
                    yValues.Add(yValues[^1]);
                    xValues.Add(xValues[^1] + rightStep);
                }
            }

            File.WriteAllText("D:\\RandomTrash\\x.txt", String.Join(' ', xValues));
            File.WriteAllText("D:\\RandomTrash\\y.txt", String.Join(' ', yValues));
        }

        private static void Graphic(List<List<Message>> dataSet)
        {
            List<Double> x = new List<Double>();
            List<Double> y = new List<Double>();
            List<Double> y2 = new List<Double>();
            for (Double i = 1; i < 2.5; i+= 0.05)
            {
                var confusionMatrix = CrossValidation.Validate(dataSet, 10, 0.015, i, 1, out var rocData);
                x.Add(i);
                y.Add(confusionMatrix.Precision(0));
                y2.Add(confusionMatrix.Precision(1));
            }

            File.WriteAllText(@"D:\RandomTrash\Bayes\x.txt", String.Join(' ', x));
            File.WriteAllText(@"D:\RandomTrash\Bayes\y.txt", String.Join(' ', y));
            File.WriteAllText(@"D:\RandomTrash\Bayes\y2.txt", String.Join(' ', y2));
        }

        private static void FindBestParams(string[] dataPartPaths)
        {
            List<Double> alphaList = new List<Double>();
            Double alpha = 0;
            for (int i = 0; i < 200; i++)
            {
                alpha += 0.005;
                alphaList.Add(alpha);
            }

            (double micro, double macro, int n, double bestAlpha) = (default, default, default, default);
            object locker = new Object();

            Parallel.For(1, 4, i =>
            {
                List<List<Message>> dataSet = dataPartPaths
                    .Select(k => ParsePart(k, i))
                    .ToList();

                Parallel.ForEach(alphaList, d =>
                {
                    var confusionMatrix = CrossValidation.Validate(dataSet, 10, d, 1, 1, out _);
                    var prec0 = confusionMatrix.MicroF1Score(1);
                    var prec1 = confusionMatrix.MacroF1Score(1);

                    lock (locker)
                    {
                        if (prec0 > micro && prec1 > macro)
                        {
                            macro = prec1;
                            micro = prec0;
                            n = i;
                            bestAlpha = d;
                        }
                    }
                });
            });

            Console.WriteLine("N=" + n);
            Console.WriteLine("Alpha=" + bestAlpha);
        }

        private static List<Message> ParsePart(String partFolderPath, int n)
        {
            List<Message> messages = new List<Message>();
 
            foreach (String dataFile in Directory.EnumerateFiles(partFolderPath))
            {
                var data = File
                    .ReadAllLines(dataFile)
                    .SelectMany(k => k
                        .Trim('\n', ' ')
                        .Split(' '))
                    .Where(k => !String.IsNullOrWhiteSpace(k))
                    .Skip(1)
                    .Select(Int32.Parse)
                    .ToArray();

                var message = new Message(data, dataFile.Contains("spmsg"), n);
                messages.Add(message);
            }

            return messages;
        }
    }
}