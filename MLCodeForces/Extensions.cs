using System;
using System.Linq;

namespace MLCodeForces
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) || Double.IsInfinity(value) ? 0 : value;
        public static Int32[] ReadNumbers(this String str) => str.Split(' ').Select(Int32.Parse).ToArray();
    }
}