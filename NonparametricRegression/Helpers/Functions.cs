using System;
using System.Linq;
using static System.Math;

namespace NonparametricRegression.Helpers
{
    public delegate Double DistanceFunction(Double[] vector1, Double[] vector2);

    public delegate Double KernelFunction(Double distance);

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
        public static KernelFunction[] KernelFunctionsWithDistanceLimitations = { Uniform, Triangular, Epanechnikov, Quartic, Triweight, Tricube, Cosine };

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
    }
}