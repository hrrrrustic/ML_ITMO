using System;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.BestFinders
{
    public class BestDistance : IComparable<BestDistance>
    {
        public readonly BestWindow BestWindow;
        public readonly DistanceFunction Distance;

        public BestDistance(BestWindow bestWindow, DistanceFunction distance) => (BestWindow, Distance) = (bestWindow, distance);

        public override String ToString() => Distance.Method.Name + " | " + BestWindow.ToString();

        public Int32 CompareTo(BestDistance other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return BestWindow.CompareTo(other.BestWindow);
        }
    }
}