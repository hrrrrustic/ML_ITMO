﻿using System;
using System.Collections.Generic;
using System.Linq;
using NonparametricRegression.Helpers;
using static NonparametricRegression.Helpers.Functions;

namespace NonparametricRegression.BestFinders
{
    public static class BestParamsFinder<T> where T : IDataSetObject
    {
        public static BestParams FindBestParams(List<T> dataRows, RegressionToClassificationFunction converter, Dictionary<DistanceFunction, Double> maxDistances)
        {
            return KernelFunctions
                .AsParallel()
                .Select(k => new BestParams(FindBestDistance(k), k))
                .OrderByDescending(k => k.BestDistance)
                .First();

            BestDistance FindBestDistance(KernelFunction kernel) => FindBestDistanceFunction(dataRows, kernel, maxDistances, converter);
        }

        public static BestDistance FindBestDistanceFunction(List<T> dataRows, KernelFunction kernel, Dictionary<DistanceFunction, Double> maxDistances, RegressionToClassificationFunction converter)
        {
            return DistanceFunctions
                .AsParallel()
                .Select(k => new BestDistance(FindBestWindow(k), k))
                .OrderByDescending(k => k.BestWindow)
                .First();
            
            BestWindow FindBestWindow(DistanceFunction distance) 
                => FindBestWindowType(new FunctionContainer(distance, kernel, converter), dataRows, maxDistances[distance]);
        }

        public static BestWindow FindBestWindowType(FunctionContainer functionContainer, List<T> dataRows, Double maxDistance)
        {
            (Double maxMicro, Double maxMacro, Window bestWindow) = (Double.MinValue, Double.MinValue, null);

            for (Int32 i = 1; i < dataRows.Count / 2; i++)
                CheckWindow(new Window(i));

            for (Double i = 0; i < maxDistance; i += 0.025)
                CheckWindow(new Window(i));

            void CheckWindow(Window window)
            {
                ConfusionMatrix confusion = new DataSet<T>(dataRows).GetConfusionMatrix(functionContainer, window);
                (Double macro, Double micro) = (confusion.MicroF1Score(1), confusion.MacroF1Score(1));

                if (micro > maxMicro && macro > maxMacro)
                    (maxMacro, maxMicro, bestWindow) = (macro, micro, window);
            }

            return new BestWindow(bestWindow, maxMicro, maxMacro);
        }
    }
}