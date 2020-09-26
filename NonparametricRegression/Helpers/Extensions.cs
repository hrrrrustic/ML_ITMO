using System;

namespace NonparametricRegression.Helpers
{
    public static class Extensions
    {
        public static Double SafeValue(this Double value) => Double.IsNaN(value) ? 0 : value;
        public static T[] ExceptIndex<T>(this T[] array, Int32 index)
        {
            Int32 listCount = array.Length;
            T[] result = new T[listCount - 1];

            Int32 newRowsIndex = 0;
            for (Int32 j = 0; j < listCount; j++)
            {
                if (j == index)
                    continue;

                result[newRowsIndex] = array[j];
                newRowsIndex++;
            }

            return result;
        }
    }
}