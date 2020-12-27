using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskK
    {
        public static void Solve()
        {
            var classCount = int.Parse(Console.ReadLine());
            var count = int.Parse(Console.ReadLine());

            var values = new Dictionary<int, List<int>>();
            for (int i = 0; i < count; i++)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers()
                    .ToArray();

                if(!values.ContainsKey(row[0]))
                    values.Add(row[0], new List<Int32>());

                values[row[0]].Add(row[1]);
            }

            var res = values
                    .Keys
                    .Sum(k => GetAvg(values[k]) * values[k].Count);

            Console.WriteLine(res / count);
        }

        private static Double GetAvg(List<int> items)
        {
            var avg = items.Average();
            return items.Average(k => Math.Pow(k - avg, 2));
        }
    }
}