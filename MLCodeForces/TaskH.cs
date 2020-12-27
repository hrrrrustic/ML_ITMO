using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskH
    {
        public static void Solve()
        {
            var objectCount = Int32.Parse(Console.ReadLine());
            List<int> x = new List<Int32>(objectCount);
            List<int> y = new List<Int32>(objectCount);
            for (int i = 0; i < objectCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers()
                    .ToArray();
                x.Add(row[0]);
                y.Add(row[1]);
            }

            Console.WriteLine(GetPearson(x, y));
        }

        private static Double GetPearson(List<int> x, List<int> y)
        {
            var xMean = Mean(x);
            var yMean = Mean(y);
            var xStd = Std(x, xMean);
            var yStd = Std(y, yMean);

            if (Math.Abs(xStd) < 1e-5 || Math.Abs(yStd) < 1e-5)
                return 0;

            var top = MeanDiff(x, xMean)
                .Zip(MeanDiff(y, yMean), (d1, d2) => d1 * d2)
                .Sum();

            return top / (xStd * yStd);
        }

        private static Double[] MeanDiff(List<Int32> value, Double avg) 
            => value.Select((k, i) => k - avg).ToArray();
        private static Double Mean(List<int> items) => items.Average();
        private static Double Std(List<int> items, double mean) =>
            Math.Sqrt(items.Select(k => Math.Pow(k - mean, 2)).Sum());
    }
}