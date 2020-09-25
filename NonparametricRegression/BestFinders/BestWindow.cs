using System;
using NonparametricRegression.Helpers;

namespace NonparametricRegression.BestFinders
{
    public class BestWindow : IComparable<BestWindow>
    {
        private readonly Window _window;
        private readonly Double _microFScore;
        private readonly Double _macroFScore;

        public BestWindow(Window window, Double microFScore, Double macroFScore) => (_window, _microFScore, _macroFScore) = (window, microFScore, macroFScore);

        public Int32 CompareTo(BestWindow other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            if (_macroFScore > other._macroFScore && _microFScore > other._microFScore)
                return 1;

            return -1;
        }

        public override String ToString() => "Micro : " + _microFScore + " Macro : " + _macroFScore + " | " + _window.ToString();
    }
}