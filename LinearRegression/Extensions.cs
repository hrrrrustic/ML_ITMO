using System;
using System.Linq;

namespace LinearRegression
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) || Double.IsInfinity(value) ? 0 : value;
        public static Double NextDouble(this Random random, Double min, Double max) => random.NextDouble() * (max - min) + min;

        public static Boolean IsSimilar(this Double[] array, Double[] another, Double eps)
        {
            for (int i = 0; i < array.Length; i++)
                if (Math.Abs(array[i] - another[i]) > eps)
                    return false;

            return true;
        }
    }
}