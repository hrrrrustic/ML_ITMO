using System;
using System.Collections.Generic;
using System.Linq;
using NonparametricRegression.Helpers;

namespace NonparametricRegression
{
    public class DataSet
    {
        private readonly DataSetObject[] _dataSet;
        private readonly Double[] _maxValues;
        private readonly Double[] _minValues;
        private readonly Int32 _featureCount;
        public Int32 Count => _dataSet.Length;
        private readonly Int32 _classCount;
        public DataSet(DataSetObject[] dataSet)
        {
            _dataSet = dataSet;
            _featureCount = dataSet.First().Features.Length;
            _maxValues = Enumerable.Repeat(Double.MinValue, _featureCount).ToArray();
            _minValues = Enumerable.Repeat(Double.MaxValue, _featureCount).ToArray();

            HashSet<Int32> classes = new HashSet<Int32>();
            foreach (var row in _dataSet)
            {
                classes.Add(row.Label);
                for (Int32 i = 0; i < _featureCount; i++)
                {
                    if (row.Features[i] < _minValues[i])
                        _minValues[i] = row.Features[i];

                    if (row.Features[i] > _maxValues[i])
                        _maxValues[i] = row.Features[i];
                }
            }
            _classCount = classes.Count;
        }

        private Double MinMax(Double min, Double max, Double current) => (current - min) / (max - min);

        public ConfusionMatrix GetConfusionMatrix(DistanceFunction distance, KernelFunction kernel, Window window, Boolean oneHot)
        {
            Int32[,] matrix = new Int32[_classCount, _classCount];

            for (Int32 i = 0; i < _dataSet.Length; i++)
            {
                Int32 predict;
                if (oneHot)
                    predict = OneHotLeaveOneOutStep(distance, kernel, window, i);
                else
                    predict = (Int32)Math.Round(LeaveOneOutStep(distance, kernel, window, i));

                if (predict > _classCount)
                    predict = _classCount - 1;

                Int32 actual = _dataSet[i].Label;

                matrix[actual, predict]++;
            }

            return new ConfusionMatrix(matrix);
        }

        public Int32 OneHotLeaveOneOutStep(DistanceFunction distance, KernelFunction kernel, Window window, Int32 controlIndex)
        {
            Double maxClassValue = Double.MinValue;
            Int32 index = Int32.MinValue;
            for (Int32 i = 0; i < _classCount; i++)
            {
                Int32 currentClass = i;

                DataSetObject[] currentClassRows = new DataSetObject[_dataSet.Length - 1];
                Int32 newRowsIndex = 0;
                for (Int32 j = 0; j < _dataSet.Length; j++) //ОпТиМиЗаЦиЯ
                {
                    if (j == controlIndex)
                        continue;

                    var rowObject = _dataSet[j];

                    currentClassRows[newRowsIndex] = new DataSetObject(rowObject.Features, rowObject.Label == currentClass ? 1 : 0);
                    newRowsIndex++;
                }

                var result = new DataSet(currentClassRows)
                    .GetPredictkNN(_dataSet[controlIndex].Features, kernel, distance, window);

                if (result > maxClassValue)
                {
                    maxClassValue = result;
                    index = i;
                }
            }

            return index;
        }

        private Double LeaveOneOutStep(DistanceFunction distance, KernelFunction kernel, Window window, Int32 controlIndex)
            => new DataSet(_dataSet.ExceptIndex(controlIndex))
                .GetPredictkNN(_dataSet[controlIndex].Features, kernel, distance, window);

        public Double GetMaxDistanceFromDataSet(DistanceFunction distance) =>
            _dataSet
                .SelectMany(k => _dataSet.Select(e => e.GetDistance(k.Features, distance)))
                .Max();

        public void Normalize()
        {
            foreach (DataSetObject row in _dataSet)
                for (Int32 i = 0; i < _featureCount; i++)
                    row.Features[i] = MinMax(_minValues[i], _maxValues[i], row.Features[i]);
        }

        public Double GetPredictkNN(Double[] requestValues, KernelFunction kernel, DistanceFunction distance, Window window)
        {
            var orderedDataSet = GetOrderedByDistance(requestValues, distance);
            return GetPredictLabel(orderedDataSet, kernel, window.GetFixedWindow(orderedDataSet));
        }

        private Double GetPredictLabel(List<(Double Distance, DataSetObject Value)> items, KernelFunction kernel, Double window)
        {
            Double sum1 = 0;
            Double sum2 = 0;

            foreach (var item in items)
            {
                if(item.Distance > window && Functions.KernelFunctionsWithDistanceLimitations.Contains(kernel))
                    break;

                var value = kernel.Invoke((item.Distance / window).SafeValue());
                sum1 += value * item.Value.Label;
                sum2 += value;
            }

            return (sum1 / sum2).SafeValue();
        }

        private List<(Double Distance, DataSetObject Value)> GetOrderedByDistance(Double[] request, DistanceFunction distance)
        => _dataSet
                .Select(k => (Distance: k.GetDistance(request, distance), k))
                .OrderBy(k => k.Distance)
                .ToList();
    }
}