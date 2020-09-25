namespace NonparametricRegression.Helpers
{
    public class FunctionContainer
    {
        public readonly DistanceFunction DistanceFunction;
        public readonly KernelFunction KernelFunction;
        public readonly RegressionToClassificationFunction ConverterFunction;

        public FunctionContainer(DistanceFunction distanceFunction, KernelFunction kernelFunction, RegressionToClassificationFunction converterFunction)
            => (DistanceFunction, KernelFunction, ConverterFunction) = (distanceFunction, kernelFunction, converterFunction);
    }
}