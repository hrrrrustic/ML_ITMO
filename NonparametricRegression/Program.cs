using System;
using System.Linq;
using Microsoft.ML;

namespace NonparametricRegression
{
    class Program
    {
        static void Main(string[] args)
        {
            MLContext context = new MLContext();
            IDataView data = context.Data.LoadFromTextFile<SeedData>("D:\\VSProjects\\ITMO\\MLCodeForces\\Lab1DataSet.csv", ',', true);
            context.Data.CreateEnumerable<SeedData>(data, false).ToList().ForEach(Console.WriteLine);
        }
    }
}
