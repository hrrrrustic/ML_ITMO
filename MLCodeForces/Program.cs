using System;
using System.Collections.Generic;
using System.Linq;

namespace MLCodeForces
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 classCount = Int32.Parse(Console.ReadLine());

            ConfusionMatrix matrix = new ConfusionMatrix(classCount);

            Console.WriteLine(matrix.MacroF1Score(1));
            Console.WriteLine(matrix.MicroF1Score(1));
        }
    }
}
