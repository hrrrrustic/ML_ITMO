using System;
using System.Collections.Generic;
using System.Linq;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.BestFinders
{
    public static class BestParamsFinder<T> where T : IDataSetObject
    {
        public static BestParams FindBestParams(List<T> dataRows, Int32 classCount, RegressionToClassificationFunction converter, Dictionary<DistanceFunction, Double> maxDistances)
        {
            return Functions
                .KernelFunctions
                .AsParallel()
                .Select(k => new BestParams(FindBestDistance(k), k))
                .OrderByDescending(k => k.BestDistance)
                .First();

            BestDistance FindBestDistance(KernelFunction kernel) => BestDistanceFinder<T>.FindBestDistanceFunction(dataRows, kernel, maxDistances, classCount, converter);
        }
    }

    public static class BestDistanceFinder<T> where T : IDataSetObject
    {
        public static BestDistance FindBestDistanceFunction(List<T> dataRows, KernelFunction kernel, Dictionary<DistanceFunction, Double> maxDistances, Int32 classCount, RegressionToClassificationFunction converter)
        {
            return Functions
                .DistanceFunctions
                .AsParallel()
                .Select(k => new BestDistance(FindBestWindow(k), k))
                .OrderByDescending(k => k.BestWindow)
                .First();

            BestWindow FindBestWindow(DistanceFunction distance)
            {
                return new BestWindowFinder<T>(dataRows, classCount, maxDistances[distance])
                    .FindBestWindow(new FunctionContainer(distance, kernel, converter));
            }
        }
    }
}