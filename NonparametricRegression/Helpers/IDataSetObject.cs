using System;

namespace NonparametricRegression.Helpers
{
    public interface IDataSetObject
    {
        Double GetDistance(Double[] request, DistanceFunction distanceFunction);
        Int32 Label { get; }
        Double[] Features { get; }
    }
}