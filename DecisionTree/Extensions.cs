using System;
using System.Collections.Generic;
using System.Linq;

namespace DecisionTrees
{
    public delegate bool Decision(DataSetObject obj);
    public static class Extensions
    {
        public static Double GetEntropy(this IReadOnlyCollection<DataSetObject> dataSet)
            => dataSet
                .GroupBy(k => k.Label)
                .Sum(k =>
                {
                    var percent = (double) k.Count() / dataSet.Count;
                    return -percent * Math.Log(percent, 2);
                });

        public static NodeSplit SplitByDecision(this IReadOnlyCollection<DataSetObject> dataSet, Decision decision)
        {
            var result = new Dictionary<bool, List<DataSetObject>>
            {
                [true] = new List<DataSetObject>(dataSet.Count / 2),
                [false] = new List<DataSetObject>(dataSet.Count / 2)
            };

            foreach (var obj in dataSet)
                result[decision.Invoke(obj)].Add(obj);

            return new NodeSplit(result[false], result[true]);
        }

        public static Int32 GetClassByMaxCount(this IReadOnlyCollection<DataSetObject> dataSet) 
            => dataSet
                .GroupBy(k => k.Label)
                .ToDictionary(x => x.Key, y => y.Count())
                .Min(k => k.Key);
        public static Double GetWeightedEntropy(this IReadOnlyCollection<DataSetObject> dataSet, Int32 allCount) 
            => dataSet.GetEntropy() * dataSet.Count / allCount;
        public static Double SafeValue(this Double value) => Double.IsNaN(value) || Double.IsInfinity(value) ? 0 : value;
    }
}