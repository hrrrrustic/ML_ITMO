using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SVM;

namespace MLCodeForces
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TaskE.Solve();
            /*int[] polynomialDegrees = {1, 2, 3, 4, 5};
            double[] coefficientLimitations = {0.05, 0.1, 0.5, 1, 5.0, 10, 50, 100};
            string path = @"D:\Development\VisualStudio\ITMO\ML\SVM\geyser.csv";

            var dataSet = File
                .ReadAllLines(path)
                .Skip(1)
                .Select(ParseRow)
                .ToList();

            dataSet = SVM.SVM.NormalizeDataSet(dataSet);*/
        }

        private static SVM.DataSetObject ParseRow(string row)
        {
            var split = row.Split(',').ToArray();
            var features = split
                .Take(split.Length - 1)
                .Select(Double.Parse)
                .ToArray();

            return new SVM.DataSetObject(features, split[^1][0]);
        }
    }
}