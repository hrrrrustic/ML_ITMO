using System;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.BestFinders
{
    public class BestParams
    {
        public readonly BestDistance BestDistance;
        public readonly KernelFunction Kernel;

        public BestParams(BestDistance bestDistance, KernelFunction kernel) => (BestDistance, Kernel) = (bestDistance, kernel);

        public override String ToString() => Kernel.Method.Name + " | " + BestDistance.ToString();
    }
}