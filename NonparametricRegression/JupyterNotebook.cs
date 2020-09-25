using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using NonparametricRegression.Data;
using XPlot.Plotly;

namespace NonparametricRegression
{
    public class JupyterNotebook
    {
        public static void Notebook()
        {
            MLContext context = new MLContext();
            IDataView data = context.Data.LoadFromTextFile<EcoliCsvData>("D:\\VSProjects\\ITMO\\ML\\ecoli.csv", ',', true);
            var dataCsvRows = context
                .Data
                .CreateEnumerable<EcoliCsvData>(data, false)
                .ToList();

            Int32[] classes = dataCsvRows
                .Select(k => k.Class)
                .Distinct()
                .ToArray();

            var dict = Enumerable
                .Range(0, classes.Length)
                .ToDictionary(k => classes[k], e => e);

            dataCsvRows.ForEach(k => k.Class = dict[k.Class]);

            var dataRows = dataCsvRows
                .Select(k => k.ToVectorData())
                .ToList();

            var matricesNaive = NonparametricRegression.Program.GetConfusionMatricesWithNaive<EcoliVectorData>(dataRows);
            var plotMarker = new Graph.Marker { size = 2 };

            var scattersNaive = new List<Graph.Scatter>
            {
                new Graph.Scatter
                {
                    name = "MicroF", marker = plotMarker, dx = 1, dy = 0.1,
                    y = matricesNaive.Select(k => k.Item2.MicroF1Score(1)),
                    x = matricesNaive.Select(k => k.Item1)
                },

                new Graph.Scatter
                {
                    name = "MicroF", marker = plotMarker, dx = 1, dy = 0.1,
                    y = matricesNaive.Select(k => k.Item2.MacroF1Score(1)),
                    x = matricesNaive.Select(k => k.Item1)
                },
            };
            var chart1 = Chart.Plot(scattersNaive);
            chart1.WithXTitle("k-vlaue");
            chart1.WithYTitle("F value");
            chart1.WithTitle("Naive Tricube Manhattan");
            //display(chart);


            var matricesOneHot = NonparametricRegression.Program.GetConfusionMatricesWithOneHot<EcoliVectorData>(dataRows);
            var scattersOneHot = new List<Graph.Scatter>
            {
                new Graph.Scatter
                {
                    name = "MicroF", dx = 1, dy = 0.1, marker = plotMarker,
                    y = matricesOneHot.Select(k => k.Item2.MicroF1Score(1)),
                    x = matricesOneHot.Select(k => k.Item1)
                },

                new Graph.Scatter
                {
                    name = "MicroF", dx = 1, dy = 0.1, marker = plotMarker,
                    y = matricesOneHot.Select(k => k.Item2.MacroF1Score(1)),
                    x = matricesOneHot.Select(k => k.Item1)
                },
            };
            var chart2 = Chart.Plot(scattersOneHot);
            chart2.WithXTitle("k-vlaue");
            chart2.WithYTitle("F value");
            chart2.WithTitle("OneHot Epanechnikov Euclidean");
            //display(chart);
        }
    }
}