using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class TaskJ
    {
        public static Int32[] ReadArray() => Console.ReadLine().Split(' ').Select(int.Parse).ToArray();

        public static (Int32, Int32) ReadPair()
        {
            Int32[] el = ReadArray();
            return (el[0], el[1]);
        }
        public static void Solve()
        {
            var classCount = int.Parse(Console.ReadLine());
            var count = int.Parse(Console.ReadLine());

            List<(Int64, Int64)> inner = Enumerable.Repeat(0, classCount).Select(_ => ((Int64)0, (Int64)0)).ToList();
            var outer = ((Int64)0, (Int64)0);

            Int64 innerSum = 0;
            Int64 outerSum = 0;

            List<(Int32 value, Int32 category)> x = new List<(Int32, Int32)>(count);
            for (Int32 i = 0; i < count; i++)
                x.Add(ReadPair());

            foreach ((Int32 value, Int32 category) v in x.OrderBy(v => v.value))
            {
                outer = (outer.Item1 + 1, outer.Item2 + v.value);
                outerSum += outer.Item1 * v.value - outer.Item2;

                inner[v.category - 1] = (inner[v.category - 1].Item1 + 1, inner[v.category - 1].Item2 + v.value);
                innerSum += inner[v.category - 1].Item1 * v.value - inner[v.category - 1].Item2;
            }

            Console.WriteLine(innerSum * 2);
            Console.WriteLine((outerSum - innerSum) * 2);
        }

        public static void Solve2()
        {
            var classCount = int.Parse(Console.ReadLine());
            var count = int.Parse(Console.ReadLine());

            List<(Int64, Int64)> inner = new List<(Int64, Int64)>();
            var outer = ((Int64)0, (Int64)0);

            Int64 innerSum = 0;
            Int64 outerSum = 0;

            List<(Int32 value, Int32 category)> x = new List<(Int32, Int32)>(count);
            for (Int32 i = 0; i < count; i++)
                x.Add(ReadPair());

            foreach ((Int32 value, Int32 category) v in x.OrderBy(v => v.value))
            {
                outer = (outer.Item1 + 1, outer.Item2 + v.value);
                outerSum += outer.Item1 * v.value - outer.Item2;

                inner[v.category - 1] = (inner[v.category - 1].Item1 + 1, inner[v.category - 1].Item2 + v.value);
                innerSum += inner[v.category - 1].Item1 * v.value - inner[v.category - 1].Item2;
            }

            Console.WriteLine(innerSum * 2);
            Console.WriteLine((outerSum - innerSum) * 2);
        }
    }
}