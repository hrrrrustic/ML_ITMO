using System;
using System.Collections.Generic;
using System.Linq;

namespace NonparametricRegression.Helpers
{
    public class Window
    {
        private readonly Double _fixedWindow;
        private readonly Int32 _variableWindow;
        public readonly Boolean IsFixed;

        public Window(Double fixedWindow) => (_fixedWindow, IsFixed) = (fixedWindow, true);

        public Window(Int32 variableWindow) => (_variableWindow, IsFixed) = (variableWindow, false);

        public Double GetFixedWindow<T>(List<(Double Distance, T Value)> orderedValues) where T : DataSetObject
        {
            if (IsFixed)
                return _fixedWindow;

            return orderedValues.ElementAt(_variableWindow).Distance;
        }

        public override String ToString()
        {
            if (IsFixed)
                return _fixedWindow + " window";

            return _variableWindow + " neighbors";
        }
    }
}