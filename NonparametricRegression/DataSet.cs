using System;
using System.Collections.Generic;
using System.Linq;
using NonparametricRegression.Helpers;

namespace NonparametricRegression
{
    public class DataSet<T> where T : IDataSetObject
    {
        private readonly List<T> _dataSet;
        private readonly Double[] _maxValues;
        private readonly Double[] _minValues;
        private readonly Int32 _featureCount;
        public Int32 Count => _dataSet.Count;
        public DataSet(List<T> dataSet)
        {
            _dataSet = dataSet;
            _featureCount = dataSet.First().Features.Length;
            _maxValues = new Double[_featureCount];
            _minValues = new Double[_featureCount];

            for (Int32 i = 0; i < _featureCount; i++)
            {
                _minValues[i] = _dataSet.Min(k => k.Features[i]);
                _maxValues[i] = _dataSet.Max(k => k.Features[i]);
            }
        }

        private Double MinMax(Double min, Double max, Double current) => Math.Abs((current - min) / (max - min));

        public ConfusionMatrix GetConfusionMatrix(FunctionContainer container, Window window)
        {
            Int32 classCount = _dataSet.Select(k => k.Label).Distinct().Count();
            Int32[,] matrix = new Int32[classCount, classCount];

            for (Int32 i = 0; i < _dataSet.Count; i++)
            {
                Dictionary<Int32, Double> result = LeaveOneOutStep(container.DistanceFunction, container.KernelFunction, window, i);
                (Int32 predict, Int32 actual) = (container.ConverterFunction.Invoke(result), _dataSet[i].Label);
                matrix[actual, predict]++;
            }

            return new ConfusionMatrix(matrix);
        }

        private Dictionary<Int32, Double> LeaveOneOutStep(DistanceFunction distance, KernelFunction kernel, Window window, Int32 controlIndex)
            => new DataSet<T>(_dataSet.ExceptIndex(controlIndex))
                .GetPredictkNN(_dataSet[controlIndex].Features, kernel, distance, window);

        public Double GetMaxDistanceFromDataSet(DistanceFunction distance) =>
            _dataSet
                .SelectMany(k => _dataSet.Select(e => e.GetDistance(k.Features, distance)))
                .Max();

        public void Normalize()
        {
            foreach (T row in _dataSet)
                for (Int32 i = 0; i < _featureCount; i++)
                    row.Features[i] = MinMax(_minValues[i], _maxValues[i], row.Features[i]);
        }

        public Dictionary<Int32, Double> GetPredictkNN(Double[] requestValues, KernelFunction kernel, DistanceFunction distance, Window window)
        {
            var orderedDataSet = GetOrderedByDistance(requestValues, distance);
            return GetPredictLabel(orderedDataSet, kernel, window.GetFixedWindow(orderedDataSet));
        }

        private Dictionary<Int32, Double> GetPredictLabel(List<(Double Distance, T Value)> items, KernelFunction kernel, Double window) =>
            items
                .Select(k => (kernel.Invoke((k.Distance / window).SafeValue()), k.Value))
                .GroupBy(k => k.Value.Label)
                .ToDictionary(k => k.Key, 
                    e => e.Sum(x => x.Item1));

        private List<(Double Distance, T Value)> GetOrderedByDistance(Double[] request, DistanceFunction distance)
        => _dataSet
                .Select(k => (Distance: k.GetDistance(request, distance), k))
                .OrderBy(k => k.Distance)
                .ToList();

        public Double[] NormalizeRequest(Double[] request)
            => request
                .Select((k, i) => MinMax(_minValues[i], _maxValues[i], request[i]))
                .ToArray();
    }
}