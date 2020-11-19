using System;

namespace MLCodeForces
{
    public class NormalizationInfo
    {
        public readonly Double[] MaxValues;
        public readonly Double[] MinValues;

        public NormalizationInfo(Double[] maxValues, Double[] minValues)
        {
            MaxValues = maxValues;
            MinValues = minValues;
        }
    }
}