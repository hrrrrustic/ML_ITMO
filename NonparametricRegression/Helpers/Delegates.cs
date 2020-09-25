using System;
using System.Collections.Generic;

namespace NonparametricRegression.Helpers
{
    public delegate Double DistanceFunction(Double[] vector1, Double[] vector2);

    public delegate Double KernelFunction(Double distance);

    public delegate Int32 RegressionToClassificationFunction(Dictionary<Int32, Double> labelValues);
}