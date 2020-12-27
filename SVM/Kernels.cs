using System;
using System.Collections.Generic;
using System.Linq;

namespace SVM
{
    public class Kernels
    {
        private static readonly int Const = 1;

        public static double Linear(List<double> values1, List<double> values2) => Polynomial(values1, values2, 1);

        public static double Polynomial(List<double> values1, List<double> values2, int power)
            => Math.Pow(values1.Select((k, i) => k * values2[i]).Sum() + Const, power);

        public static double GaussianRadiant(List<double> values1, List<double> values2, double gamma)
            => Math
                .Exp(-gamma * values1
                    .Select((k, i) => Math.Pow(k - values2[i], 2))
                    .Sum());
    }
}