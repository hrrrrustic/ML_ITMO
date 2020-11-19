using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LinearRegression
{
    public static class LinearRegression
    {
        private static readonly Random Random = new Random();
        public static Double[] GetWeightsWithGradient(List<DataSetObject> dataSet, Double regularizationLambda, int stepCount, bool needNormalization = false)
        {
            if (needNormalization)
                dataSet = Normalize(dataSet);

            return GradientDescent(dataSet, regularizationLambda, stepCount);
        }

        public static Double[] GetWeightsWithMnk(List<DataSetObject> dataSet, Double regularizationLambda, bool needNormalization = false)
        {
            if (needNormalization)
                dataSet = Normalize(dataSet);

            return MnkWithPseudoInverseMatrix(dataSet, regularizationLambda);
        }

        public static List<DataSetObject> Normalize(List<DataSetObject> dataSet)
        {
            (Double[] max, Double[] min) = GetMaxAndMin(dataSet);

            foreach (var obj in dataSet)
                for (int i = 0; i < obj.Features.Length; i++)
                {
                    if (max[i] == min[i])
                        obj.Features[i] = 1;
                    else
                        obj.Features[i] = MinMax(obj.Features[i], i);
                }

            return dataSet;

            Double MinMax(double value, int i) => (value - min[i]) / (max[i] - min[i]);
        }

        private static (Double[], Double[]) GetMaxAndMin(List<DataSetObject> dataSet)
        {
            Double[] min = Enumerable.Repeat(Double.MaxValue, dataSet[0].Features.Length).ToArray();
            Double[] max = Enumerable.Repeat(Double.MinValue, min.Length).ToArray();

            foreach (var obj in dataSet)
                for (int i = 0; i < obj.Features.Length; i++)
                {
                    Double feature = obj.Features[i];
                    if (feature > max[i])
                        max[i] = feature;

                    if (feature < min[i])
                        min[i] = feature;
                }

            return (max, min);
        }

        private static Double[] GradientDescent(List<DataSetObject> dataSet, Double regularizationLambda, int stepCount)
        {
            var smallValue = (double)1 / (2 * dataSet[0].Features.Length);
            Double[] weights = new Double[dataSet.First().Features.Length];
            int batchPercentage = 40;
            Double[] prevWeights = new Double[weights.Length];
            weights.CopyTo(prevWeights, 0);
            for (int i = 0; i < stepCount; i++)
            {
                var learningRate = (double)5 / (i + 1);

                BatchGradientDescentStep(weights, dataSet, learningRate, batchPercentage, regularizationLambda);

                weights.CopyTo(prevWeights, 0);
            }

            return weights;
        }

        private static void BatchGradientDescentStep(double[] weights, List<DataSetObject> dataSet, Double learningRate, int batchPercentage, double lambda)
        {
            Double[] gradient = new Double[weights.Length];
            int batch = batchPercentage * dataSet.Count / 100;

            for(int j = 0; j < batch; j++)
            {
                var ind = Random.Next(0, dataSet.Count);
                var obj = dataSet[ind];
                var predict = obj.GetPredict(weights);
                var error = obj.MseDerivative(predict, dataSet.Count);

                for (int i = 0; i < obj.Features.Length; i++)
                    gradient[i] += error * obj.Features[i] + LassoRegularization(weights[i], lambda);
            }

            for (int i = 0; i < weights.Length; i++)
                weights[i] = weights[i] * (1 - learningRate * lambda) - learningRate * gradient[i];

        }

        private static Double LassoRegularization(Double weight, Double lambda) => Math.Abs(weight) * lambda;
        private static Double LassoRegularization2(Double[] weight, Double lambda) => weight.Sum(Math.Abs) * lambda;

        public static Double GetSmape(List<DataSetObject> dataSet, Double[] weights) 
            => dataSet.Sum(obj => SmapeStep(obj.GetPredict(weights), obj.Label)) / dataSet.Count;

        private static Double SmapeStep(Double predict, Double actual) => Math.Abs(predict - actual) / (Math.Abs(predict) + Math.Abs(actual));

        public static Double[] MnkWithPseudoInverseMatrix(List<DataSetObject> dataSet, Double lambda)
        {
            Matrix<Double> features = DenseMatrix.OfRowArrays(dataSet.Select(k => k.Features));
            var identityMatrix = DenseMatrix.CreateIdentity(features.ColumnCount) * lambda;

            var help = dataSet.Select(k => k.Label).ToArray();
            Matrix<Double> values = DenseMatrix.OfColumnArrays(new List<Double[]>{help});

            var transposeFeatures = features.Transpose();

            var weights = (transposeFeatures * features + identityMatrix).Inverse() * transposeFeatures * values;

            return weights.Column(0).ToArray();
        }
    }
}