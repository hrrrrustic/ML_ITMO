using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskI
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

            Console.WriteLine(GetSpearman(x, y));
        }

        private static Double GetSpearman(List<int> x, List<int> y)
        {
            var xRanks = GetRank(x);
            var yRanks = GetRank(y);

            Double rankDiffSum = 0;

            for (int i = 0; i < x.Count; i++)
                rankDiffSum += Math.Pow(xRanks[i] - yRanks[i], 2);

            int n = x.Count;
            return 1 - 6 * rankDiffSum / (n * (Math.Pow(n, 2) - 1));
        }

        private static Int32[] GetRank(List<int> items)
        {
            var sorted = items
                .Select((k, i) => (item: k, i))
                .OrderBy(k => k.item)
                .ToList();
            
            int[] ranks = new Int32[items.Count];
            int rank = 0;
            for (int i = 1; i < sorted.Count; i++)
            {
                if (sorted[i - 1].item != sorted[i].item)
                    rank++;

                ranks[sorted[i].i] = rank;
            }

            return ranks;
        }
    }
}