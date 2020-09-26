using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using NonparametricRegression.BestFinders;
using NonparametricRegression.Helpers;
using static NonparametricRegression.Helpers.Functions;

namespace NonparametricRegression
{
    public class Program
    {
        public static void RunBestFinder()
        {
            MLContext context = new MLContext();
            IDataView data = context.Data.LoadFromTextFile<EcoliCsvData>("D:\\VSProjects\\ITMO\\ML\\ecoli.csv", ',', true);
            var dataCsvRows = context
                .Data
                .CreateEnumerable<EcoliCsvData>(data, false)
                .ToList();

            Int32[] classes = dataCsvRows
                .Select(k => k.Class)
                .Distinct()
                .ToArray();

            var dict = Enumerable
                .Range(0, classes.Length)
                .ToDictionary(k => classes[k], e => e);
            dataCsvRows.ForEach(k => k.Class = dict[k.Class]);

            var dataRows = dataCsvRows
                .Select(k => k.ToDataSetObject())
                .ToArray();

            var dataSet = new DataSet(dataRows);

            Dictionary<DistanceFunction, Double> maxDistances = DistanceFunctions
                .ToDictionary(k => k,
                    e => dataSet.GetMaxDistanceFromDataSet(e));

            (KernelFunction, DistanceFunction, BestWindow) naiveResult = BestParamsFinder.FindBestParams(dataSet, maxDistances, false);
            Console.WriteLine(naiveResult.Item2.Method.Name + " | " + naiveResult.Item2.Method.Name + " | " + naiveResult.Item3);
            (KernelFunction, DistanceFunction, BestWindow) oneHotResult = BestParamsFinder.FindBestParams(dataSet, maxDistances, true);
            Console.WriteLine(oneHotResult.Item2.Method.Name + " | " + oneHotResult.Item2.Method.Name + " | " + oneHotResult.Item3);
        }

        public static List<(Int32, ConfusionMatrix)> GetConfusionMatricesWithOneHot(List<DataSetObject> dataRows) 
            => GetMatrices(new DataSet(dataRows.ToArray()), EuclideanDistance, Tricube, true);

        public static List<(Int32, ConfusionMatrix)> GetConfusionMatricesWithNaive(List<DataSetObject> dataRows) 
            => GetMatrices(new DataSet(dataRows.ToArray()), ManhattanDistance, Tricube, false);
            
        private static List<(Int32, ConfusionMatrix)> GetMatrices(DataSet dataSet, DistanceFunction distance, KernelFunction kernel, Boolean oneHot) 
            => Enumerable
            .Range(0, dataSet.Count - 1)
            .AsParallel()
            .AsOrdered()
            .Select(k => 
                (k, dataSet.GetConfusionMatrix(distance, kernel, new Window(k), oneHot)))
            .ToList();
    }
}
