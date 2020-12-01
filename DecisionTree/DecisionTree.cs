using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DecisionTrees
{
    public class DecisionTree
    {
        private readonly DecisionTreeItem _head;
        public Int32 TreeHeight => _head.InnerNodesCount;
        private DecisionTree(DecisionTreeItem head)
        {
            _head = head;
        }

        public static DecisionTree BuildFromDataSet(DataSetObject[] dataSet, int maxHeight)
        {
            var headNodeDecision = DecisionTreeItem.GetBestDecisionFromDataSet(dataSet, out NodeSplit split);
            var headNode = InitializeTreeItem(headNodeDecision, dataSet, split, maxHeight);
            return new DecisionTree(headNode);
        }

        private static DecisionTreeItem InitializeTreeItem(Decision decision, IReadOnlyCollection<DataSetObject> dataSet, NodeSplit curSplit, int levelAvailableCount)
        {
            var label = dataSet.First().Label;
            if(dataSet.All(k => k.Label == label))
                return new DecisionTreeLeaf(dataSet.First().Label);

            if(levelAvailableCount == 1)
              return new DecisionTreeLeaf(dataSet.GetClassByMaxCount());

            levelAvailableCount--;

            var leftDecision = DecisionTreeItem.GetBestDecisionFromDataSet(curSplit.LeftPart, out var leftSplit);
            var leftChild = InitializeTreeItem(leftDecision, curSplit.LeftPart, leftSplit, levelAvailableCount);

            var rightDecision = DecisionTreeItem.GetBestDecisionFromDataSet(curSplit.RightPart, out var rightSplit);
            var rightChild = InitializeTreeItem(rightDecision, curSplit.RightPart, rightSplit, levelAvailableCount);

            return new DecisionTreeNode(leftChild, rightChild, decision, dataSet.GetClassByMaxCount());
        }

        public Int32 ClassifyObject(DataSetObject obj, int maxHeight) => _head.ClassifyObject(obj, maxHeight);
    }
}