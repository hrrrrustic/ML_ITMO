using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public class TaskC
    {
        public class FunctionsResolver
        {
            private static readonly Dictionary<String, Action> KernelFunctions = new Dictionary<String, Action>();

            private static readonly Dictionary<String, Func<Int32[], Int32[], Double>> DistanceFunctions = new Dictionary<String, Func<Int32[], Int32[], Double>>
            {
                ["euclidean"] = EuclideanDistance,
                ["manhattan"] = ManhattanDistance,
                ["chebyshev"] = ChebyshevDistance
            };

            private static Double EuclideanDistance(Int32[] vector1, Int32[] vector2)
            {
                Int32 sum = GetSum(vector1, vector2, (x, y) => (x - y) * (x - y));
                return Math.Sqrt(sum);
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

                Double maxValue = Double.MinValue;

                for (Int32 i = 0; i < vector1.Length; i++)
                {
                    Int32 value = Math.Abs(vector2[i] - vector1[i]);
                    if (value > maxValue)
                        maxValue = value;
                }

                return maxValue;
            }


            public static Func<Int32[], Int32[], Double> ResolveKernel(String name) => DistanceFunctions[name];

            public Func<Int32[], Int32[], Double> ResolveDistance(String name) => DistanceFunctions[name];

            public void ResolveWindow(String name)
            {

            }
        }

        public void Solve()
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

            String distanceFunction = Console.ReadLine();
            String kernelFunction = Console.ReadLine();
            String windowFunction = Console.ReadLine();
            Int32 windowValue = Int32.Parse(Console.ReadLine());
        }
    }

}