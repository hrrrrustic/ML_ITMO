using System;
using System.Collections.Generic;

namespace DecisionTrees
{
    internal abstract class DecisionTreeItem
    {
        internal abstract int ClassifyObject(DataSetObject obj, int availableHeight);
        internal abstract Int32 InnerNodesCount { get; }

        protected readonly int DecisionResult;

        protected DecisionTreeItem(Int32 decisionResult)
        {
            DecisionResult = decisionResult;
        }

        internal static Decision GetBestDecisionFromDataSet(IReadOnlyCollection<DataSetObject> dataSet, out NodeSplit splitResult)
        {
             var currentEntropy = dataSet.GetEntropy();
             double minEntropy = currentEntropy;
            Decision bestDecision = k => true;
            splitResult = new NodeSplit(dataSet, Array.Empty<DataSetObject>());
            foreach (var obj in dataSet)
            {
                for (int i = 0; i < obj.Features.Length; i++)
                {
                    int j = i;
                    Decision lessDecision = k => obj.Features[j] < k.Features[j];

                    var split = dataSet.SplitByDecision(lessDecision);
                    var lessDecisionEntropy = split.GetWeightedEntropy();

                    if (lessDecisionEntropy < minEntropy)
                        (minEntropy, bestDecision, splitResult) = (lessDecisionEntropy, lessDecision, split);
                }
            }

            return bestDecision;
        }
    }

    internal class DecisionTreeNode : DecisionTreeItem
    {
        private readonly DecisionTreeItem _leftChild;
        private readonly DecisionTreeItem _rightChild;
        private readonly Decision _decision;
        internal override Int32 InnerNodesCount => 1 + Math.Max(_leftChild.InnerNodesCount, _rightChild.InnerNodesCount);
        internal DecisionTreeNode(DecisionTreeItem leftChild, DecisionTreeItem rightChild, Decision decision, int decisionResult) : base(decisionResult)
        {
            _leftChild = leftChild;
            _rightChild = rightChild;
            _decision = decision;
        }

        internal override Int32 ClassifyObject(DataSetObject obj, int availableHeight)
        {
            if (availableHeight == 1)
                return DecisionResult;

            availableHeight--;

            if (_decision.Invoke(obj))
                return _rightChild.ClassifyObject(obj, availableHeight);

            return _leftChild.ClassifyObject(obj, availableHeight);
        }

    }

    internal class DecisionTreeLeaf : DecisionTreeItem
    {
        internal override Int32 InnerNodesCount => 1;

        internal DecisionTreeLeaf(Int32 decisionResult) : base(decisionResult){}

        internal override Int32 ClassifyObject(DataSetObject obj, Int32 availableHeight) => DecisionResult;
    }
}