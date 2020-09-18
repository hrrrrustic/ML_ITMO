using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class DoubleExtensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) ? 0 : value;
    }
    public class ObjectDescription
    {
        private readonly Int32[] _coordinates;

        public readonly Int32 Value;

        public ObjectDescription(Int32[] coordinates, Int32 value)
        {
            _coordinates = coordinates;
            Value = value;
        }

        public Double GetDistance(Int32[] requestVector, Func<Int32[], Int32[], Double> function) => function.Invoke(_coordinates, requestVector);
    }
    class Program
    {
        static void Main(string[] args)
        {
            TaskC.Solve();
        }
    }

    public class TaskC
    {
        public class FunctionsResolver
        {
            private static readonly Dictionary<String, Func<Double, Double>> KernelFunctions = new Dictionary<String, Func<Double, Double>>
            {
                ["uniform"] = Uniform,
                ["triangular"] = Triangular,
                ["epanechnikov"] = Epanechnikov,
                ["quartic"] = Quartic,
                ["triweight"] = Triweight,
                ["tricube"] = Tricube,
                ["gaussian"] = Gaussian,
                ["сosine"] = Cosine,
                ["logistic"] = Logistic,
                ["sigmoid"] = Sigmoid
            };


            private static readonly Dictionary<String, Func<Int32[], Int32[], Double>> DistanceFunctions = new Dictionary<String, Func<Int32[], Int32[], Double>>
            {
                ["euclidean"] = EuclideanDistance,
                ["manhattan"] = ManhattanDistance,
                ["chebyshev"] = ChebyshevDistance
            };

            private static Double EuclideanDistance(Int32[] vector1, Int32[] vector2)
            {
                Int32 sum = GetSum(vector1, vector2, (x, y) => (x - y) * (x - y));
                return Math.Sqrt(sum).SafeValue();
            }

            private static Double ManhattanDistance(Int32[] vector1, Int32[] vector2) => GetSum(vector1, vector2, (x, y) => Math.Abs(x - y));

            private static Int32 GetSum(Int32[] vector1, Int32[] vector2, Func<Int32, Int32, Int32> func)
            {
                if (vector1.Length != vector2.Length)
                    throw new ArgumentException();

                Int32 sum = 0;

                for (Int32 i = 0; i < vector1.Length; i++)
                    sum += func.Invoke(vector1[i], vector2[i]);

                return sum;
            }
            private static Double ChebyshevDistance(Int32[] vector1, Int32[] vector2)
            {
                if (vector1.Length != vector2.Length)
                    throw new ArgumentException();

                Int32 maxValue = Int32.MinValue;

                for (Int32 i = 0; i < vector1.Length; i++)
                {
                    Int32 value = Math.Abs(vector2[i] - vector1[i]);
                    if (value > maxValue)
                        maxValue = value;
                }

                return maxValue;
            }

            private static Double Uniform(Double distance) => distance >= 1 ? 0 : distance / 2;

            private static Double Triangular(Double distance) => distance >= 1 ? 0 : 1 - Math.Abs(distance);
            private static Double Epanechnikov(Double distance) => distance >= 1 ? 0 : 0.75 * (1 - distance * distance);
            private static Double Quartic(Double distance) => distance >= 1 ? 0 : 15 * (1 - distance * distance) * (1 - distance * distance) / 16;
            private static Double Triweight(Double distance) => distance >= 1 ? 0 : 35 * (1 - distance * distance) * (1 - distance * distance) * (1 - distance * distance) / 32;
            private static Double Tricube(Double distance) => distance >= 1 ? 0 : 70 * Math.Pow(1 - Math.Abs(distance * distance * distance), 3) / 81;
            private static Double Gaussian(Double distance) => Math.Pow(Math.E, -0.5 * distance * distance) / Math.Sqrt(2 * Math.PI);
            private static Double Cosine(Double distance) => distance >= 1 ? 0 : 0.25 * Math.PI * Math.Cos(Math.PI * distance * 0.5);
            private static Double Logistic(Double distance) => 1 / (Math.Pow(Math.E, distance) + 2 + Math.Pow(Math.E, -distance));
            private static Double Sigmoid(Double distance) => 2 / (Math.PI * (Math.Pow(Math.E, distance) + Math.Pow(Math.E, -distance)));
            public static Func<Double, Double> ResolveKernel(String name) => KernelFunctions[name];

            public static Func<Int32[], Int32[], Double> ResolveDistance(String name) => DistanceFunctions[name];

        }

        public static void Solve()
        {
            Int32[] args = Console
                .ReadLine()
                .Split(' ')
                .Select(Int32.Parse)
                .ToArray();

            Int32 objectCount = args[0];
            Int32 featureCount = args[1];
            List<ObjectDescription> objects = new List<ObjectDescription>(objectCount);
            for (Int32 i = 0; i < objectCount; i++)
            {
                Int32[] objectValues = Console
                    .ReadLine()
                    .Split(' ')
                    .Select(Int32.Parse)
                    .ToArray();

                ObjectDescription objectDescription = new ObjectDescription(objectValues.Take(objectValues.Length - 1).ToArray(), objectValues.Last());
                objects.Add(objectDescription);
            }

            Int32[] requestFeatures = Console
                .ReadLine()
                .Split(' ')
                .Select(Int32.Parse)
                .ToArray();

            String distanceFunctionName = Console.ReadLine();
            String kernelFunctionName = Console.ReadLine();
            String windowFunctionName = Console.ReadLine();
            Int32 windowValue = Int32.Parse(Console.ReadLine());

            List<(Double distance, ObjectDescription objectDescription)> distances = new List<(Double, ObjectDescription)>(objects.Count);

            Func<Int32[], Int32[], Double> distanceFunction = FunctionsResolver.ResolveDistance(distanceFunctionName);
            Func<Double, Double> kernelFunction = FunctionsResolver.ResolveKernel(kernelFunctionName);

            foreach (ObjectDescription description in objects)
            {
                Double distance = description.GetDistance(requestFeatures, distanceFunction);
                distances.Add((distance, description));
            }

            distances = distances.OrderBy(k => k.distance).ToList();

            Double result = 0;

            if (windowFunctionName == "fixed")
                result = FixedWindow(distances, windowValue, kernelFunction);
            else
                result = VariableWindow(distances, windowValue, kernelFunction);

            Console.WriteLine(result);

        }

        public static Double FixedWindow(List<(Double distance, ObjectDescription objectDescription)> sortedObjects, Int32 window, Func<Double, Double> kernelFunction)
        {
            Double sum1 = 0;
            Double sum2 = 0;
            for (Int32 i = 0; i < sortedObjects.Count; i++)
            {
                if (sortedObjects[i].distance > window)
                    break;

                var h = sortedObjects[i].distance / window;
                var val = kernelFunction.Invoke(h.SafeValue());
                sum1 += val * sortedObjects[i].objectDescription.Value;
                sum2 += val;
            }

            return (sum1 / sum2).SafeValue();
        }

        public static Double VariableWindow(List<(Double distance, ObjectDescription objectDescription)> sortedObjects, Int32 window, Func<Double, Double> kernelFunction)
        {
            Double sum1 = 0;
            Double sum2 = 0;
            for (Int32 i = 0; i < sortedObjects.Count; i++)
            {
                if (i >= window)
                    break;

                var h = sortedObjects[i].distance / sortedObjects[window].distance;
                var val = kernelFunction.Invoke(h.SafeValue());
                sum1 += val * sortedObjects[i].objectDescription.Value;
                sum2 += val;
            }

            return (sum1 / sum2).SafeValue();
        }
    }
}
