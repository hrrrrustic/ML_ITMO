using System;
using System.Linq;

namespace LinearRegression
{
    public class DataSetObject
    {
        public readonly Double[] Features;
        public readonly Double Label;

        public DataSetObject(Double[] features, Double label)
        {
            Features = features;
            Label = label;
        }

        public Double GetPredict(Double[] weights) => Features.Select((t, i) => t * weights[i]).Sum();
        
        public Double MseDerivative(Double predict, int count) => 2 * (predict - Label) / count;
    }
}