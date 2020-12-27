using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MLCodeForces
{
    public class TaskD
    {
        public static readonly Random Random = new Random();
        public class DataSetObject
        {
            public readonly Double[] Features;
            public Double Label { get; set; }

            public DataSetObject(Double[] features, Double label)
            {
                Features = features;
                Label = label;
            }

            public Double GetPredict(Double[] weights) => Features.Select((t, i) => weights[i] * t).Sum();
            public Double MseDerivative(Double predict, int dataSetCount) => 2 * (predict - Label);
            public Double SmapeDerivative(Double predict, int dataSetCount) => (FirstPart(predict) - SecondPart(predict)) / dataSetCount;

            private Double FirstPart(Double predict) 
                => Math.Sign(predict - Label) / (Math.Abs(predict) + Math.Abs(Label));
            private Double SecondPart(Double predict) 
                => Math.Sign(predict) * Math.Abs(predict - Label) / (Math.Pow(Math.Abs(predict) + Math.Abs(Label), 2));
        }

        private static Double GetSmape(List<DataSetObject> dataSet, Double[] weights)
        {
            Double sum = 0;
            foreach (var obj in dataSet)
            {
                var predict = obj.GetPredict(weights);
                var actual = obj.Label;

                sum += Math.Abs(predict - actual) / (Math.Abs(predict) + Math.Abs(actual));
            }

            return sum / dataSet.Count;
        }

        private static (Double[] avg, Double[] std) MeanNormalization(List<DataSetObject> dataSet)
        {
            Double[] avg = new Double[dataSet[0].Features.Length + 1];

            foreach (var dataSetObject in dataSet)
            {
                for (int i = 0; i < dataSetObject.Features.Length; i++)
                    avg[i] += dataSetObject.Features[i];

                avg[^1] += dataSetObject.Label;
            }

            for (int i = 0; i < avg.Length; i++)
                avg[i] /= dataSet.Count;

            Double[] std = new Double[avg.Length];

            foreach (var dataSetObject in dataSet)
            {
                for (int i = 0; i < dataSetObject.Features.Length; i++)
                    std[i] += Math.Pow(dataSetObject.Features[i] - avg[i], 2);

                std[^1] += Math.Pow(dataSetObject.Label - avg[^1], 2);
            }

            for (int i = 0; i < std.Length; i++)
            {
                std[i] /= dataSet.Count - 1;
                std[i] = Math.Sqrt(std[i]);
            }

            foreach (var dataSetObject in dataSet)
            {
                for (int i = 0; i < dataSetObject.Features.Length; i++)
                {
                    if (std[i] == 0)
                    {
                        dataSetObject.Features[i] -= avg[i];
                        continue;
                    }

                    dataSetObject.Features[i] = Mean(i, dataSetObject.Features[i]);
                }
                
                if(std[^1] != 0)
                    dataSetObject.Label = Mean(avg.Length - 1, dataSetObject.Label);
            }

            return (avg, std);

            Double Mean(int i, Double value) => (value - avg[i]) / std[i];
        }

        public static void Solve()
        {
            //Int32[] taskInfo = Console.ReadLine().ReadNumbers().ToArray();
            //Int32 objectCount = taskInfo[0];
            //Int32 featureCount = taskInfo[1];
            string path = @"D:\RandomTrash\MLCF\DTest\0.40_0.65.txt";
            var data = File
                .ReadAllLines(path)
                .Select(k => k
                    .Split(' ')
                    .Select(Int32.Parse)
                    .ToArray())
                .ToArray();
            var featureCount = data[0][0];
            var objectCount = data[1][0];
            Double[] weights = new Double[featureCount + 1];

            List<DataSetObject> dataSet = new List<DataSetObject>(objectCount);
            for (Int32 i = 2; i < objectCount + 2; i++)
            {
                //Int32[] row = Console.ReadLine().ReadNumbers().ToArray();
                var features = data[i]
                    .Take(featureCount)
                    .Append(1)
                    .Select(k => (double)k)
                    .ToArray();

                DataSetObject currentObject = new DataSetObject(features, data[i].Last());
                dataSet.Add(currentObject);
            }

            var save = dataSet
                .Select(k => new DataSetObject(k
                    .Features
                    .Select(e => e)
                    .ToArray(), k.Label))
                .ToList();

            var info = MeanNormalization(dataSet);
            /*dataSet
                .SelectMany(k => k.Features.Select(k => k))
                .OrderByDescending(k => k)
                .ToList()
                .ForEach(Console.WriteLine);*/

            Double learningRate = 0.005;
            for (Int32 i = 0; i < 550; i++)
            {
                GradientDescent(dataSet, weights, learningRate, (int)(0.2 * dataSet.Count));
                Console.WriteLine(GetSmape(dataSet, weights));
                learningRate *= 0.992;
            }
            weights = DenormalizeWeights(weights, info.avg, info.std);
            var smape = GetSmape(save, weights);
            Console.WriteLine("SMAPE : " + smape);
            Console.WriteLine(String.Join(" ", weights));
        }

        private static Double[] DenormalizeWeights(Double[] weights, Double[] avg, Double[] std)
        {
            Double[] helper = new Double[weights.Length];
            for (int i = 0; i < weights.Length - 1; i++)
            {
                if(std[i] == 0)
                    continue;
                
                helper[i] = -weights[i] * avg[i] / std[i];
                weights[i] = weights[i] * std[^1] / std[i];
            }

            weights[^1] = (helper.Sum() + weights[^1]) * std[^1] + avg[^1];
            return weights;
        }
        private static void GradientDescent(List<DataSetObject> dataSet, Double[] weights, Double learningRate, int batchSize)
        {
            Span<Double> gradient = new Double[weights.Length];
            for (Int32 i = 0; i < batchSize; i++)
            {
                DataSetObject currentObject = dataSet[Random.Next(0, dataSet.Count)];
                Double predict = currentObject.GetPredict(weights);
                Double error = currentObject.MseDerivative(predict, dataSet.Count).SafeValue();

                for (int j = 0; j < currentObject.Features.Length; j++)
                    gradient[j] += error * currentObject.Features[j]; // + Lasso(gradient[j], 0.00003);
            }

            for (int j = 0; j < weights.Length; j++)
                weights[j] = weights[j] * (1 - learningRate * 0.05) - learningRate * gradient[j];
        }
        private static Double Lasso(Double weight, Double lambda) => Math.Abs(weight) * lambda;
    }
}