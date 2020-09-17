using System;
using System.Linq;

namespace MLCodeForces
{
    public class TaskB
    {
        public static void Solve()
        {
            Int32 classCount = Int32.Parse(Console.ReadLine());
            
            ConfusionMatrix matrix = new ConfusionMatrix(classCount);
            
          
            Console.WriteLine(matrix.MacroF1Score(1));
            Console.WriteLine(matrix.MicroF1Score(1));
        }
    }
}