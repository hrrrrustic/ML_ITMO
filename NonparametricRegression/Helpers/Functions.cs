using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace NonparametricRegression.Helpers
{
    public static class Functions
    {
        public static readonly DistanceFunction[] DistanceFunctions = {EuclideanDistance, ManhattanDistance, ChebyshevDistance};

        public static Double EuclideanDistance(Double[] vector1, Double[] vector2)
            => Math
                .Sqrt(vector1
                    .Zip(vector2, (d1, d2) => Math.Pow(d1 - d2, 2))
                    .Sum())
                .SafeValue();

        public static Double ManhattanDistance(Double[] vector1, Double[] vector2)
            => vector1
                .Zip(vector2, (d1, d2) => Math.Abs(d1 - d2))
                .Sum()
                .SafeValue();

        public static Double ChebyshevDistance(Double[] vector1, Double[] vector2)
            => vector1
                .Zip(vector2, (d1, d2) => Math.Abs(d1 - d2))
                .Max()
                .SafeValue();

        public static KernelFunction[] KernelFunctions = { Uniform, Triangular, Epanechnikov, Quartic, Triweight, Tricube, Gaussian, Cosine, Logistic, Sigmoid };
        public static Double Uniform(Double distance) => distance >= 1 ? 0 : 0.5;
        public static Double Triangular(Double distance) => distance >= 1 ? 0 : 1 - Abs(distance);
        public static Double Epanechnikov(Double distance) => distance >= 1 ? 0 : 0.75 * (1 - Pow(distance, 2));
        public static Double Quartic(Double distance) => distance >= 1 ? 0 : 15 * Pow(1 - Pow(distance, 2), 2) / 16;
        public static Double Triweight(Double distance) => distance >= 1 ? 0 : 35 * Pow(1 - Pow(distance, 2), 3) / 32;
        public static Double Tricube(Double distance) => distance >= 1 ? 0 : 70 * Pow(1 - Abs(Pow(distance, 3)), 3) / 81;
        public static Double Gaussian(Double distance) => Pow(E, -0.5 * Pow(distance, 2)) / Sqrt(2 * PI);
        public static Double Cosine(Double distance) => distance >= 1 ? 0 : 0.25 * PI * Cos(PI * distance * 0.5);
        public static Double Logistic(Double distance) => 1 / (Pow(E, distance) + 2 + Pow(E, -distance));
        public static Double Sigmoid(Double distance) => 2 / (PI * (Pow(E, distance) + Pow(E, -distance)));

        public static RegressionToClassificationFunction[] RegressionToClassificationsFunctions = { SimpleRound, OneHot };

        public static Int32 SimpleRound(Dictionary<Int32, Double> labelValues)
        {
            Double distanceSum = labelValues.Sum(k => k.Value);
            var result = labelValues.Keys.Sum(k => k * labelValues[k]) / distanceSum;
            Int32 classificationResult = (Int32)Math.Round(result.SafeValue());

            return classificationResult;
        }

        public static Int32 OneHot(Dictionary<Int32, Double> labelValues)
            => labelValues
                .OrderByDescending(k => k.Value)
                .First()
                .Key;
    }
}