using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace MLCodeForces
{
    public class TaskD
    {
        public class DataSetObject
        {
            public readonly Double[] Features;
            public readonly Int32 Label;

            public DataSetObject(Double[] features, Int32 label)
            {
                Features = features;
                Label = label;
            }

            public Double GetTrainPredict(Double[] weights) 
                => weights.Last() + Features.Select((t, i) => weights[i] * t).Sum();

            public Double SmapeDerivative(Double predict) => FirstPart(predict) - SecondPart(predict);

            private Double FirstPart(Double predict)
            {
                return Math.Sign(predict - Label) / (Math.Abs(predict) + Math.Abs(Label));
            }
            private Double SecondPart(Double predict)
            {
                return Math.Sign(predict) * Math.Abs(predict - Label) / (Math.Pow(Math.Abs(predict) + Math.Abs(Label), 2));
            }
            public override String ToString()
            {
                return String.Join(" ", Features) + Label;
            }
        }

        public static void Solve()
        {
            Int32[] taskInfo = Console.ReadLine().ReadNumbers();
            Int32 objectCount = taskInfo[0];
            Int32 featureCount = taskInfo[1];
            Double[] weights = {-1 / 2 * objectCount, 1 / 2 * objectCount};
            List<DataSetObject> dataSet = new List<DataSetObject>(objectCount);
            for (Int32 i = 0; i < objectCount; i++)
            {
                Int32[] row = Console.ReadLine().ReadNumbers();
                DataSetObject currentObject = new DataSetObject(row.Take(featureCount).Select(k => (double)k).ToArray(), row.Last());
                dataSet.Add(currentObject);
            }

            //var info = MinMaxNormalize(dataSet);
            Double learningRate = 1.7;

            for (Int32 i = 1; i < 2000; i++)
            {
                Console.WriteLine(String.Join(" ", weights));

                GradientDescent(dataSet, weights, learningRate);
                learningRate *= 0.998;
            }
            Console.WriteLine(String.Join(" ", weights));

        }

        private static NormalizationInfo MinMaxNormalize(List<DataSetObject> dataSet)
        {
            Double[] max = new Double[dataSet.First().Features.Length];
            Double[] min = new Double[max.Length];

            foreach (var dataSetObject in dataSet)
                for (int i = 0; i < dataSetObject.Features.Length; i++)
                    CheckMinMax(i, dataSetObject.Features[i]);

            foreach (var dataSetObject in dataSet)
                for (int i = 0; i < dataSetObject.Features.Length; i++)
                    dataSetObject.Features[i] = MinMax(i, dataSetObject.Features[i]);

            return new NormalizationInfo(max, min);

            void CheckMinMax(int i, Double value)
            {
                if (max[i] < value)
                    max[i] = value;

                if (min[i] > value)
                    min[i] = value;
            }

            Double MinMax(int i, Double value) => (value - min[i]) / (max[i] - min[i]);
        }
        private static void GradientDescent(List<DataSetObject> dataSet, Double[] weights, Double learningRate)
        {
            Span<Double> gradient = new Double[weights.Length];
            for (Int32 i = 0; i < dataSet.Count; i++)
            {
                DataSetObject currentObject = dataSet[i];
                Double predict = currentObject.GetTrainPredict(weights);
                Double error = currentObject.SmapeDerivative(predict).SafeValue();

                for (int j = 0; j < currentObject.Features.Length; j++)
                    gradient[j] += error * currentObject.Features[j];

                gradient[^1] += error;
            }

            for (int j = 0; j < weights.Length; j++)
            {
                gradient[j] /= dataSet.Count;
                weights[j] += -learningRate * gradient[j];
            }
        }
    }
}