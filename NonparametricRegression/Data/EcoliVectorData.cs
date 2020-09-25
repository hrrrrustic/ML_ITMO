using System;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.Data
{
    public class EcoliVectorData : IDataSetObject
    {
        public EcoliVectorData(Int32 label, Double[] features) => (Label, Features) = (label, features);

        public Int32 Label { get; }
        public Double[] Features { get; }
        public Double GetDistance(Double[] request, DistanceFunction distanceFunction) => distanceFunction.Invoke(Features, request);
        public override String ToString() => String.Join(" ", Features) + Label;
    }
}