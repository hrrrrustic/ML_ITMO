using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using NonparametricRegression.BestFinders;
using NonparametricRegression.Data;
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
                .Select(k => k.ToVectorData())
                .ToList();

            var dataSet = new DataSet<EcoliVectorData>(dataRows);

            var maxDistances = new Dictionary<DistanceFunction, Double>();
            foreach (var distanceFunction in DistanceFunctions)
                maxDistances.Add(distanceFunction, dataSet.GetMaxDistanceFromDataSet(distanceFunction));

            foreach (RegressionToClassificationFunction function in RegressionToClassificationsFunctions)
            {
                BestParams result = BestParamsFinder<EcoliVectorData>.FindBestParams(dataRows, classes.Length, function, maxDistances);
                Console.WriteLine(result);
            }
        }
        public static List<(Int32, ConfusionMatrix)> GetConfusionMatricesWithOneHot<T>(List<T> dataRows)
            where T : IDataSetObject =>
            GetMatrices(new DataSet<T>(dataRows), new FunctionContainer(EuclideanDistance, Tricube, OneHot));

        public static List<(Int32, ConfusionMatrix)> GetConfusionMatricesWithNaive<T>(List<T> dataRows)
            where T : IDataSetObject =>
            GetMatrices(new DataSet<T>(dataRows), new FunctionContainer(ManhattanDistance, Tricube, SimpleRound));
            
        private static List<(Int32, ConfusionMatrix)> GetMatrices<T>(DataSet<T> dataSet, 
            FunctionContainer container)
            where T : IDataSetObject => 
            Enumerable
            .Range(0, dataSet.Count)
            .Select(k => 
                (k, dataSet.GetConfusionMatrix(container.DistanceFunction, container.KernelFunction, container.ConverterFunction, new Window(k))))
            .ToList();
    }
}
