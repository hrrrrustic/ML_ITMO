using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public class ObjectDescription
    {
        private readonly Int32[] _coordinates;

        public readonly Int32 Value;

        public ObjectDescription(Int32[] coordinates, Int32 value)
        {
            _coordinates = coordinates;
            Value = value;
        }

        public Double GetDistance(Int32[] requestVector, Func<Int32[], Int32[], Double> function) => function.Invoke(_coordinates, requestVector);
    }
}   