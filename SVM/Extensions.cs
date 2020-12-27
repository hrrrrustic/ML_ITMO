using System;

namespace SVM
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) || Double.IsInfinity(value) ? 0 : value;
    }
}