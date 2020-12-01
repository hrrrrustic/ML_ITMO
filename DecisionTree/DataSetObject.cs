using System;

namespace DecisionTrees
{
    public class DataSetObject
    {
        public DataSetObject(Double[] features, Int32 label) => (Label, Features) = (label, features);
        public Int32 Label { get; }
        public Double[] Features { get; }
    }
}