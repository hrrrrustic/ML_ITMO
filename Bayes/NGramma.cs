using System;
using System.Collections.Generic;
using System.Linq;

namespace Bayes
{
    public class NGramma<T> : IEquatable<NGramma<T>>
    {
        public readonly T[] Values;
        private readonly Int32 _count;

        public NGramma(params T[] values)
        {
            Values = values;
            _count = values.Length;
        }

        public bool Equals(NGramma<T> other)
        {
            if (other is null)
                return false;

            if (_count != other._count)
                return false;

            return Values.SequenceEqual(other.Values);
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (T value in Values)
                    hash = hash * 31 + value.GetHashCode();

                return hash;
            }
        }

        public static NGramma<T>[] GetNGrammaFromArray(T[] array, int n)
        {
           return array.Select((k, i) => new NGramma<T>(array.Skip(i).Take(n).ToArray())).ToArray();
        }
    }
}