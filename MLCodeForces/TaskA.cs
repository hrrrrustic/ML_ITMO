using System;
using System.Linq;

namespace MLCodeForces
{
    public class TaskA
    {
        public static void Solve()
        {
            Int32[] info = Console
                .ReadLine()
                .Split(' ')
                .Select(Int32.Parse)
                .ToArray();

            Int32 partsCount = info[2];

            Console
                .ReadLine()
                .Split(' ')
                .Select((k, i) => (Class: Int32.Parse(k), Index: i))
                .OrderBy(k => k.Class)
                .Select((k, i) => (Class: k.Class, Index: k.Index, ClassIndex: i))
                .GroupBy(k => k.ClassIndex % partsCount, e => e.Index + 1)
                .ToList()
                .ForEach(k =>
                    Console.WriteLine($"{k.Count()} " + String.Join(" ", k)));
        }
    }
}