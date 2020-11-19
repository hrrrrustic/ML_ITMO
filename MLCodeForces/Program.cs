using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinearRegression;
using XPlot.Plotly;

namespace MLCodeForces
{
    class Program
    {
        static void Main(string[] args)
        {
           

            var rows = File.ReadAllLines(@"D:\RandomTrash\linear\3.txt");

            int objectCount = int.Parse(rows[1]);
            var dataSet = rows
                .Skip(2)
                .Take(objectCount)
                .Select(ParseRow)
                .ToList();

            var trainCount = Int32.Parse(rows[2 + objectCount]);
            var trainDataSet = rows
                .Skip(2 + objectCount + 1)
                .Take(trainCount)
                .Select(ParseRow)
                .ToList();

            trainDataSet = LinearRegression.LinearRegression.Normalize(trainDataSet);
            dataSet = LinearRegression.LinearRegression.Normalize(dataSet);

            var weights = LinearRegression.LinearRegression.GetWeightsWithGradient(dataSet, 0.001, 200);
            var result = LinearRegression.LinearRegression.GetSmape(trainDataSet, weights);
            Console.WriteLine(result);
        }

        private static DataSetObject ParseRow(String str)
        {
            var row = str.Split(' ').Select(Double.Parse).ToArray();
            return new DataSetObject(row.Take(row.Length - 1).ToArray(), row[^1]);
        }
    }
}