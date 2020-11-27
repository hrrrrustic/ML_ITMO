using System;
using System.Collections.Generic;

namespace Bayes
{
    public static class Extensions
    { 
        public static Double SafeValue(this Double value) => Double.IsNaN(value) ? 0 : value;
    }
}