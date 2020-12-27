using System;
using System.Collections.Generic;
using System.Linq;
using SVM;

namespace DecisionTrees
{
    public class ConfusionMatrix
    {
        private readonly List<Int32> _indexes;
        private readonly Int32[] TP, TN, FP, FN, C, P;
        private readonly Int32 _actualCount;

        public ConfusionMatrix(Int32[,] matrix)
        {
            Int32 classCount = matrix.GetLength(0);
            (TP, TN) = (new Int32[classCount], new Int32[classCount]);
            (FP, FN) = (new Int32[classCount], new Int32[classCount]);
            (C, P) = (new Int32[classCount], new Int32[classCount]);

            for (Int32 i = 0; i < classCount; i++)
                for (Int32 j = 0; j < classCount; j++)
                {
                    C[i] += matrix[i, j];
                    P[j] += matrix[i, j];
                    _actualCount += matrix[i, j];

                    if (i == j)
                        TP[i] = matrix[i, j];
                    else
                    {
                        FP[i] += matrix[i, j];
                        FN[j] += matrix[i, j];
                    }
                }

            _indexes = Enumerable.Range(0, classCount).ToList();
        }

        public Int32 TotalPredictCount => TP.Sum() + TN.Sum() + FN.Sum() + FP.Sum();

        private Double PrecisionW() =>
            _indexes
                .Select(k => ((Double)TP[k] * C[k] / P[k]).SafeValue())
                .Sum() / _actualCount;

        private Double RecallW() => 
            _indexes
                .Select(k => ((Double)TP[k]).SafeValue())
                .Sum() / _actualCount;

        public Double Precision(Int32 classIndex)
            => ((Double)TP[classIndex] / (TP[classIndex] + FP[classIndex])).SafeValue();

        private Double Recall(Int32 classIndex)
            => ((Double)TP[classIndex] / (TP[classIndex] + FN[classIndex])).SafeValue();

        private Double F1Score(Double beta, Int32 classIndex)
        {
            (Double prec, Double rec) = (Precision(classIndex), Recall(classIndex));
            return 2 * prec * rec / (beta * beta * prec + rec).SafeValue();
        }

        public double Accuracy() => ((double)TP.Sum() + TN.Sum()) / TotalPredictCount;
        public Double MacroF1Score(Double beta)
        {
            (Double prec, Double rec) = (PrecisionW(), RecallW());
            return ((1 + beta * beta) * prec * rec / (beta * beta * prec + rec)).SafeValue();
        }

        public Double MicroF1Score(Double beta) =>
            _indexes
                .Select(k => (C[k] * F1Score(beta, k)).SafeValue())
                .Sum() / _actualCount;
    }
}