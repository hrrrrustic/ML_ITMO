using System;
using System.Collections.Generic;

namespace DecisionTrees
{
    public class NodeSplit
    {
        public readonly IReadOnlyCollection<DataSetObject> LeftPart;
        public readonly IReadOnlyCollection<DataSetObject> RightPart;

        public NodeSplit(IReadOnlyCollection<DataSetObject> leftPart, IReadOnlyCollection<DataSetObject> rightPart)
        {
            LeftPart = leftPart;
            RightPart = rightPart;
        }

        public Double GetWeightedEntropy()
        {
            var allCount = LeftPart.Count + RightPart.Count;

            return LeftPart.GetWeightedEntropy(allCount) + RightPart.GetWeightedEntropy(allCount);
        }
    }
}