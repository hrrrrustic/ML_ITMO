using System;
using System.Collections.Generic;
using System.Linq;

namespace SVM
{
    public static class SVM
    {
        public static List<DataSetObject> NormalizeDataSet(List<DataSetObject> dataSet)
        {
            (Double[] max, Double[] min) = GetMaxAndMin(dataSet);

            foreach (var obj in dataSet)
                for (int i = 0; i < obj.Features.Length; i++)
                {
                    if (max[i] == min[i])
                        obj.Features[i] = 1;
                    else
                        obj.Features[i] = MinMax(obj.Features[i], i);
                }

            return dataSet;

            Double MinMax(double value, int i) => (value - min[i]) / (max[i] - min[i]);
        }

        private static (Double[], Double[]) GetMaxAndMin(List<DataSetObject> dataSet)
        {
            Double[] min = Enumerable.Repeat(Double.MaxValue, dataSet[0].Features.Length).ToArray();
            Double[] max = Enumerable.Repeat(Double.MinValue, min.Length).ToArray();

            foreach (var obj in dataSet)
                for (int i = 0; i < obj.Features.Length; i++)
                {
                    Double feature = obj.Features[i];
                    if (feature > max[i])
                        max[i] = feature;

                    if (feature < min[i])
                        min[i] = feature;
                }

            return (max, min);
        }
    }
}
