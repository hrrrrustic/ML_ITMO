using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MLCodeForces
{
    public class ConfusionMatrix
    {
        private readonly Int32[][] _matrix;

        private readonly List<Int32> _indexes;
        private readonly Int32[] TP;
        private readonly Int32[] TN;
        private readonly Int32[] FP;
        private readonly Int32[] FN;
        private readonly Int32[] C;
        private readonly Int32[] P;

        private readonly Int32 _actualCount;

        public ConfusionMatrix(Int32 classCount)
        {
            _indexes = Enumerable.Range(0, classCount).ToList();
            _matrix = new Int32[classCount][];
            TP = new Int32[classCount];
            TN = new Int32[classCount];
            FP = new Int32[classCount];
            FN = new Int32[classCount];
            C = new Int32[classCount];
            P = new Int32[classCount];

            for (Int32 i = 0; i < classCount; i++)
            {
                _matrix[i] = Console
                    .ReadLine()
                    .Split(' ')
                    .Select(Int32.Parse)
                    .ToArray();

                for (int j = 0; j < classCount; j++)
                {
                    C[i] += _matrix[i][j];
                    P[j] += _matrix[i][j];
                    _actualCount += _matrix[i][j];

                    if (i == j)
                        TP[i] = _matrix[i][j];
                    else
                    {
                        FP[i] += _matrix[i][j];
                        FN[j] += _matrix[i][j];
                    }
                }
            }
        }

        public Double PrecisionW() => 
            _indexes
            .Select(k => ((Double) TP[k] * C[k] / P[k]).SafeValue())
            .Sum() / _actualCount;

        public Double RecallW() => _indexes
            .Select(k => ((Double) TP[k]).SafeValue())
            .Sum() / _actualCount;

        public Double Precision(Int32 classIndex)
            => ((Double) TP[classIndex] / (TP[classIndex] + FP[classIndex])).SafeValue();

        public Double Recall(Int32 classIndex)
            => ((Double) TP[classIndex] / (TP[classIndex] + FN[classIndex])).SafeValue();



        public Double F1Score(Double beta, Int32 classIndex)
        {
            Double prec = Precision(classIndex);
            Double recc = Recall(classIndex);

            return 2 * prec * recc / (beta * beta * prec + recc).SafeValue();
        }

        public Double MacroF1Score(Double beta)
        {
            Double prec = PrecisionW();
            Double recc = RecallW();

            return ((1 + beta * beta) * prec * recc / (beta * beta * prec + recc)).SafeValue();
        }

        public Double MicroF1Score(Double beta) => 
            _indexes
                .Select(k => (C[k] * F1Score(beta, k)).SafeValue())
                .Sum() / _actualCount;
    }
}