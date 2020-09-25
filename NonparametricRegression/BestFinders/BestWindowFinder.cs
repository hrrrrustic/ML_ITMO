using System;
using System.Collections.Generic;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.BestFinders
{
    public class BestWindowFinder<T> where T : IDataSetObject
    {
        private readonly List<T> _dataRows;
        private readonly Int32 _classCount;
        private readonly Double _maxDistance;

        public BestWindowFinder(List<T> dataRows, Int32 classCount, Double maxDistance) =>
            (_dataRows, _classCount, _maxDistance) = (dataRows, classCount, maxDistance);

        public BestWindow FindBestWindow(FunctionContainer functionContainer)
        {
            (Double maxMicro, Double maxMacro, Window bestWindow) = (Double.MinValue, Double.MinValue, null);

            for (Int32 i = 1; i < _dataRows.Count / 2; i++)
                CheckWindow(new Window(i));

            for (Double i = 0; i < _maxDistance; i += 0.025)
                CheckWindow(new Window(i));

            void CheckWindow(Window window)
            {
                (Double macro, Double micro) = GetFScore(window, functionContainer);
                if (micro > maxMicro && macro > maxMacro)
                    (maxMacro, maxMicro, bestWindow) = (macro, micro, window);
            }

            return new BestWindow(bestWindow, maxMicro, maxMacro);
        }

        private (Double Micro, Double Macro) GetFScore(Window window, FunctionContainer container)
        {
            ConfusionMatrix confusion = LeaveOneOut(window, container);
            return (confusion.MicroF1Score(1), confusion.MacroF1Score(1));
        }
        private ConfusionMatrix LeaveOneOut(Window window, FunctionContainer container)
        {
            Int32[,] matrix = new Int32[_classCount, _classCount];

            for (Int32 i = 0; i < _dataRows.Count; i++)
            {
                Dictionary<Int32, Double> result = LeaveOneOutStep(i, window, container);
                (Int32 predict, Int32 actual) = (container.ConverterFunction.Invoke(result), _dataRows[i].Label);
                matrix[actual, predict]++;
            }

            return new ConfusionMatrix(matrix);
        }

        private Dictionary<Int32, Double> LeaveOneOutStep(Int32 controlIndex, Window window, FunctionContainer container) 
            => new DataSet<T>(_dataRows.ExceptIndex(controlIndex))
                .GetPredictkNN(_dataRows[controlIndex].Features, container.KernelFunction, container.DistanceFunction, window);
    }
}