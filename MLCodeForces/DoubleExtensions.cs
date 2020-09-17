using System;

namespace MLCodeForces
{
    public static class DoubleExtensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) ? 0 : value;
    }
}