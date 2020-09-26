using System;

namespace NonparametricRegression.Helpers
{
    public class DataSetObject
    {
        public DataSetObject(Double[] features, Int32 label) => (Label, Features) = (label, features);
        public Double GetDistance(Double[] request, DistanceFunction distanceFunction) => distanceFunction.Invoke(request, Features);
        public Int32 Label { get; }
        public Double[] Features { get; }
    }
}