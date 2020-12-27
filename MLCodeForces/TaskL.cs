using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskL
    {
        public static void Solve()
        {
            var _ = Console
                .ReadLine();

            var objectCount = Int32.Parse(Console.ReadLine());

            Dictionary<(int, int), int> p = new Dictionary<(Int32, Int32), int>(objectCount);
            Dictionary<int, int> px1 = new Dictionary<Int32, Int32>();
            Dictionary<int, int> px2 = new Dictionary<Int32, Int32>();

            for (int i = 0; i < objectCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers();

                var x = row[0] - 1;
                var y = row[1] - 1;
                if(!p.ContainsKey((x, y)))
                    p.Add((x, y), 0);

                p[(x, y)]++;

                if(!px1.ContainsKey(x))
                    px1.Add(x, 0);

                px1[x]++;

                if (!px2.ContainsKey(y))
                    px2.Add(y, 0);

                px2[y]++;
            }

            double result = objectCount;
            foreach (var entry in p)
            {
                var expected = objectCount * px1[entry.Key.Item1] * px2[entry.Key.Item2] / Math.Pow(objectCount, 2);
                expected = expected.SafeValue();
                result += (Math.Pow(entry.Value - expected, 2) / expected).SafeValue();
                result -= expected;
            }

            Console.WriteLine(result);
        }
    }
}