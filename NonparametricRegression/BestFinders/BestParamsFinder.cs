using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NonparametricRegression.Helpers;
using static NonparametricRegression.Helpers.Functions;

namespace NonparametricRegression.BestFinders
{
    public static class BestParamsFinder<T> where T : IDataSetObject
    {
        public static (KernelFunction, DistanceFunction, BestWindow) FindBestParams(DataSet<T> dataSet, RegressionToClassificationFunction converter, Dictionary<DistanceFunction, Double> maxDistances) 
        {
            ConcurrentBag<(KernelFunction, DistanceFunction, BestWindow)> windows = new ConcurrentBag<(KernelFunction, DistanceFunction, BestWindow)>();
            Parallel.ForEach(KernelFunctions, (function, state) =>
                Parallel.ForEach(DistanceFunctions, (distanceFunction, loopState) =>
                {
                    var currentWindow = FindBestWindowType(new FunctionContainer(distanceFunction, function, converter), dataSet, maxDistances[distanceFunction]);
                    windows.Add((function, distanceFunction, currentWindow));
                }));

            return windows
                .OrderByDescending(k => k.Item3)
                .First();
        }

        public static BestWindow FindBestWindowType(FunctionContainer functionContainer, DataSet<T> dataSet, Double maxDistance)
        {
            (Double maxMicro, Double maxMacro, Window bestWindow) = (Double.MinValue, Double.MinValue, null);

            for (Int32 i = 1; i < dataSet.Count / 2; i++)
                CheckWindow(new Window(i));

            for (Double i = 0; i < maxDistance; i += 0.25)
                CheckWindow(new Window(i));

            void CheckWindow(Window window)
            {
                ConfusionMatrix confusion = dataSet.GetConfusionMatrix(functionContainer, window);
                (Double macro, Double micro) = (confusion.MicroF1Score(1), confusion.MacroF1Score(1));

                if (micro > maxMicro && macro > maxMacro)
                    (maxMacro, maxMicro, bestWindow) = (macro, micro, window);
            }

            return new BestWindow(bestWindow, maxMicro, maxMacro);
        }
    }
}