using System;
using System.Collections.Generic;
using System.Linq;

namespace NonparametricRegression.Helpers
{
    public class Window
    {
        private readonly Double _fixedWindow;
        private readonly Int32 _variableWindow;
        private readonly Boolean _isFixed;

        public Window(Double fixedWindow) => (_fixedWindow, _isFixed) = (fixedWindow, true);

        public Window(Int32 variableWindow) => (_variableWindow, _isFixed) = (variableWindow, false);

        public Double GetFixedWindow<T>(List<(Double Distance, T Value)> orderedValues) where T : IDataSetObject
        {
            if (_isFixed)
                return _fixedWindow;

            return orderedValues.ElementAt(_variableWindow).Distance;
        }

        public override String ToString()
        {
            if (_isFixed)
                return _fixedWindow + " window";

            return _variableWindow + " neighbors";
        }
    }
}