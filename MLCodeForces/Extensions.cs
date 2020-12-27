using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) || Double.IsInfinity(value) ? 0 : value;
        public static int[] ReadNumbers(this String str) => str.Split(' ').Select(Int32.Parse).ToArray();
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static Boolean IsSimilar(this Double[] array, Double[] another, Double eps)
        {
            for (int i = 0; i < array.Length; i++)
                if (Math.Abs(array[i] - another[i]) > eps)
                    return false;

            return true;
        }
    }
}