using System;
using System.Collections.Generic;

namespace NonparametricRegression.Helpers
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) ? 0 : value;
        public static List<T> ExceptIndex<T>(this List<T> list, Int32 index)
        {
            Int32 listCount = list.Count;
            T[] result = new T[listCount - 1]; 
            list.CopyTo(0, result, 0, index);
            list.CopyTo(index + 1, result, index, listCount - 1 - index);
            return new List<T>(result);
        }
    }
}