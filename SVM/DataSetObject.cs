using System;
using System.Linq;

namespace SVM
{
    public class DataSetObject
    {
        public readonly Double[] Features;
        public readonly char Label;

        public DataSetObject(Double[] features, char label)
        {
            Features = features;
            Label = label;
        }
    }
}