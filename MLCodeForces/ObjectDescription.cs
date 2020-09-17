using System;

namespace MLCodeForces
{
    public class ObjectDescription
    {
        private readonly Int32[] _coordinates;

        private readonly Int32 _value;

        public ObjectDescription(Int32[] coordinates, Int32 value)
        {
            _coordinates = coordinates;
            _value = value;
        }
    }
}