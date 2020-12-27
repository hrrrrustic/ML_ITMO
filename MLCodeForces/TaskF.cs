using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    public delegate bool Decision(DataSetObject obj);

    public class DataSetObject
    {
        public readonly int[] Features;
        public readonly int Label;
        public DataSetObject(int[] features, int label)
        {
            Features = features;
            Label = label;
        }
    }

    public class DataSet
    {
        private readonly Dictionary<Int32, List<DataSetObject>> _data;
        public Int32 ObjectCount => Objects.Count;
        public Int32 FeatureCount { get; }
        public Int32 ClassCount { get; }
        public List<DataSetObject> Objects { get; }
        public DataSet(Dictionary<Int32, List<DataSetObject>> data)
        {
            _data = data;
            ClassCount = data.Keys.Count;
            if (data.Count == 0)
            {
                FeatureCount = 0;
                Objects = new List<DataSetObject>(0);
            }
            else
            {
                FeatureCount = data.First().Value.First().Features.Length;
                Objects = data.Values.SelectMany(k => k).ToList();
            }
        }

        public Int32 GetClassByMaxCount()
        {
            (int maxCount, int result) = (Int32.MinValue, 0);

            foreach (var values in _data)
                if (values.Value.Count > maxCount)
                    (maxCount, result) = (values.Value.Count, values.Key);

            return result;
        }

        public Double GetEntropy() =>
            _data
                .Sum(k =>
                {
                    var percent = (double)k.Value.Count / ObjectCount;
                    return -percent * Math.Log(percent, 2);
                });

        public Double GetWeightedEntropy(Int32 allCount)
            => GetEntropy() * ObjectCount / allCount;

        public (DataSet left, DataSet right) SplitByDecision(Decision decision)
        {
            var result = new Dictionary<bool, Dictionary<int, List<DataSetObject>>>
            {
                [true] = new Dictionary<int, List<DataSetObject>>(ObjectCount / 2),
                [false] = new Dictionary<int, List<DataSetObject>>(ObjectCount / 2)
            };

            foreach (var obj in Objects)
            {
                var decisionResult = decision.Invoke(obj);
                if (!result[decisionResult].ContainsKey(obj.Label))
                    result[decisionResult].Add(obj.Label, new List<DataSetObject>());

                result[decisionResult][obj.Label].Add(obj);
            }

            return (new DataSet(result[true]), new DataSet(result[false]));
        }
    }

    public class NodeSplit
    {
        public readonly DataSet LeftPart;
        public readonly DataSet RightPart;
        public readonly int SplitItemIndex;
        public readonly Double SplitValue;
        public NodeSplit(DataSet leftPart, DataSet rightPart, Int32 splitItemIndex, Double splitValue)
        {
            LeftPart = leftPart;
            RightPart = rightPart;
            SplitItemIndex = splitItemIndex;
            SplitValue = splitValue;
        }

        public Double GetWeightedEntropy()
        {
            var allCount = LeftPart.ObjectCount + RightPart.ObjectCount;
            return LeftPart.GetWeightedEntropy(allCount) + RightPart.GetWeightedEntropy(allCount);
        }
    }

    public class DecisionTree
    {
        private readonly DecisionTreeItem _head;
        public Int32 NodeCount { get; }

        private DecisionTree(DecisionTreeItem head, int nodeCount)
        {
            _head = head;
            NodeCount = nodeCount;
        }

        public static DecisionTree BuildFromDataSet(DataSet dataSet, int maxHeight, bool useId3)
        {
            int initIndex = 0;
            var headNode = InitializeTreeItem(dataSet, maxHeight, Enumerable.Range(0, dataSet.FeatureCount).ToList(), useId3, ref initIndex);
            return new DecisionTree(headNode, initIndex);
        }

        private static DecisionTreeItem InitializeTreeItem(DataSet dataSet, int levelAvailableCount, 
            List<int> unusedFeatures, bool useId3, ref int nodeIndex)
        {
            nodeIndex++;
            var currentIndex = nodeIndex;

            if (dataSet.ClassCount == 1 || levelAvailableCount == 1 || unusedFeatures.Count == 0)
                return new DecisionTreeLeaf(dataSet.GetClassByMaxCount(), currentIndex);

            levelAvailableCount--;

            (var _, var usedFeature) = DecisionTreeItem.GetBestDecisionFromDataSet(dataSet, unusedFeatures, out var split);
            if(useId3)
                unusedFeatures.Remove(usedFeature);

            if(split is null || split.RightPart.ObjectCount == 0 || split.LeftPart.ObjectCount == 0)
                return new DecisionTreeLeaf(dataSet.GetClassByMaxCount(), currentIndex);

            var leftChild = InitializeTreeItem(split.LeftPart, levelAvailableCount, unusedFeatures, useId3, ref nodeIndex);
            var rightChild = InitializeTreeItem(split.RightPart, levelAvailableCount, unusedFeatures, useId3, ref nodeIndex);
            
            return new DecisionTreeNode(leftChild, rightChild, split.SplitItemIndex, split.SplitValue, currentIndex);
        }

        public void PrintTree()
        {
            _head.PrintTree();
        }

        internal abstract class DecisionTreeItem
        {
            internal readonly int NodeIndex;
            protected DecisionTreeItem(Int32 nodeIndex)
            {
                NodeIndex = nodeIndex;
            }

            public abstract void PrintTree();

            internal static (Decision, int featureIndex) GetBestDecisionFromDataSet(DataSet dataSet, 
                List<int> unusedFeatures, 
                out NodeSplit splitResult)
            {
                var currentEntropy = dataSet.GetEntropy();
                double minEntropy = currentEntropy;
                int featureIndex = 0;
                Decision bestDecision = k => true;
                splitResult = null!;
                foreach (Int32 unusedFeature in unusedFeatures)
                {
                    int j = unusedFeature;
                    int prevValue = Int32.MinValue;
                    foreach (var obj in dataSet.Objects.OrderBy(k => k.Features[j]))
                    {
                        if(obj.Features[j] == prevValue)
                            continue;

                        prevValue = obj.Features[j];
                        Decision lessDecision = l => l.Features[j] < obj.Features[j] + 0.5;

                        var parts = dataSet.SplitByDecision(lessDecision);
                        var split = new NodeSplit(parts.left, parts.right, unusedFeature, obj.Features[j] + 0.5);
                        var lessDecisionEntropy = split.GetWeightedEntropy();

                        if (lessDecisionEntropy < minEntropy)
                            (minEntropy, bestDecision, splitResult, featureIndex) = (lessDecisionEntropy, lessDecision, split, unusedFeature);

                        if(minEntropy == 0)
                            return (bestDecision, featureIndex);

                    }
                }

                return (bestDecision, featureIndex);
            }
        }

        internal class DecisionTreeNode : DecisionTreeItem
        {
            private readonly DecisionTreeItem _leftChild;
            private readonly DecisionTreeItem _rightChild;
            private readonly int _decisionItemIndex;
            private readonly Double _compareValue;
            public override void PrintTree()
            {
                Console.WriteLine($"Q {_decisionItemIndex + 1} {_compareValue} {_leftChild.NodeIndex} {_rightChild.NodeIndex}");
                _leftChild.PrintTree();
                _rightChild.PrintTree();
            }

            internal DecisionTreeNode(DecisionTreeItem leftChild, DecisionTreeItem rightChild, int itemDecisionItemIndex, Double compareValue, int nodeIndex) : base(nodeIndex)
            {
                _leftChild = leftChild;
                _rightChild = rightChild;
                _decisionItemIndex = itemDecisionItemIndex;
                _compareValue = compareValue;
            }
        }

        internal class DecisionTreeLeaf : DecisionTreeItem
        {
            private readonly int _result;
            public override void PrintTree()
            {
                Console.WriteLine($"C {_result}");
            }

            internal DecisionTreeLeaf(Int32 result, int nodeIndex) : base(nodeIndex)
            {
                _result = result;
            }
        }

    }
    public static class TaskF
    {
        /*public static int[] ReadNumbers(this String str) => str.Split(' ').Select(Int32.Parse).ToArray();

        static void Main(string[] args)
        {
            TaskF.Solve();
        }*/
        public static void Solve()
        {
            var info = Console
                .ReadLine()
                .ReadNumbers()
                .ToArray();

            var featureCount = info[0];
            var classCount = info[1];
            var maxHeight = info[2] + 1;
  
            var objectCount = Int32.Parse(Console.ReadLine());

            if (classCount == 1)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers();

                Console.WriteLine($"C {row[^1]}");
                return;
            }
            Dictionary<int, List<DataSetObject>> dataSet = new Dictionary<Int32, List<DataSetObject>>(classCount);
            for (int i = 0; i < objectCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .ReadNumbers();

                var label = row[^1];
                var obj = new DataSetObject(row.Take(featureCount).ToArray(), label);
                if(!dataSet.ContainsKey(label))
                    dataSet.Add(label, new List<DataSetObject>());

                dataSet[label].Add(obj);
            }

            if (maxHeight > 7)
                maxHeight = 6;
            var tree = DecisionTree.BuildFromDataSet(new DataSet(dataSet), maxHeight, objectCount > 200);
            Console.WriteLine(tree.NodeCount);
            tree.PrintTree();
        }
    }
}