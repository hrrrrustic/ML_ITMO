using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskM
    {
        public static void Solve()
        {
            var diffs = Console
                .ReadLine()
                .ReadNumbers()
                .ToArray();

            Span<int> p = stackalloc int[diffs[0]];
            var values = new Dictionary<(int, int), int>();
            var objectCount = Int32.Parse(Console.ReadLine());

            for (int i = 0; i < objectCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers()
                    .ToArray();

                if (!values.ContainsKey((row[0] - 1, row[1] - 1)))
                    values.Add((row[0] - 1, row[1] - 1), 0);

                values[(row[0] - 1, row[1] - 1)]++;
                p[row[0] - 1]++;
            }

            double conditionalEntropy = 0;
            foreach (var (i, j) in values.Keys)
            {
                if (values[(i, j)] == 0)
                    continue;

                conditionalEntropy += Math.Log((double)values[(i, j)] / p[i]) * values[(i, j)];
            }

            Console.WriteLine(-conditionalEntropy / objectCount);
        }
    }
}