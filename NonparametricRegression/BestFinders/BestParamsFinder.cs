using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NonparametricRegression.Helpers;
using static NonparametricRegression.Helpers.Functions;

namespace NonparametricRegression.BestFinders
{
    public static class BestParamsFinder
    {
        private static readonly Object Locker = new Object();
        public static (KernelFunction, DistanceFunction, BestWindow) FindBestParams(DataSet dataSet, Dictionary<DistanceFunction, Double> maxDistances, Boolean oneHot) 
        {
            List<(KernelFunction, DistanceFunction, BestWindow)> windows = new List<(KernelFunction, DistanceFunction, BestWindow)>();
            Parallel.ForEach(KernelFunctions, (function, state) =>
                Parallel.ForEach(DistanceFunctions, (distanceFunction, loopState) =>
                {
                    var currentWindow = FindBestWindowType(distanceFunction, function, dataSet, maxDistances[distanceFunction], oneHot);

                    lock (Locker)
                    {
                        windows.Add((function, distanceFunction, currentWindow));
                    }
                }));

            return windows
                .OrderByDescending(k => k.Item3)
                .First();
        }

        public static BestWindow FindBestWindowType(DistanceFunction distance, KernelFunction kernel, DataSet dataSet, Double maxDistance, Boolean oneHot)
        {
            (Double maxMicro, Double maxMacro, Window bestWindow) = (Double.MinValue, Double.MinValue, null);

            for (Int32 i = 1; i < (Int32)(dataSet.Count / 2); i++) //Тут тоже надо шаманить от размера
                CheckWindow(new Window(i));

            for (Double i = 0; i < maxDistance; i += 0.5) //Шаг надо менять от размера датасета
               CheckWindow(new Window(i));

            void CheckWindow(Window window)
            {
                ConfusionMatrix confusion = dataSet.GetConfusionMatrix(distance, kernel, window, oneHot);
                (Double macro, Double micro) = (confusion.MicroF1Score(1), confusion.MacroF1Score(1));

                if (micro > maxMicro && macro > maxMacro)
                    (maxMacro, maxMicro, bestWindow) = (macro, micro, window);
            }

            Console.WriteLine(distance.Method.Name + " | " + kernel.Method.Name + " | " + bestWindow.IsFixed + " | Micro : " + maxMicro + " | Macro " + maxMacro);

            return new BestWindow(bestWindow, maxMicro, maxMacro);
        }
    }
}