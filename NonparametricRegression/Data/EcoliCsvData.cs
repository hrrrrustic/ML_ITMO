using System;
using Microsoft.ML.Data;

namespace NonparametricRegression.Data
{
    public class EcoliCsvData
    {
        [LoadColumn(0)] public Double mcg { get; set; }
        [LoadColumn(1)] public Double gvh { get; set; }
        [LoadColumn(2)] public Double lip { get; set; }
        [LoadColumn(3)] public Double chg { get; set; }
        [LoadColumn(4)] public Double aac { get; set; }
        [LoadColumn(5)] public Double alm1 { get; set; }
        [LoadColumn(6)] public Double alm2 { get; set; }
        [LoadColumn(7)] public Int32 Class { get; set; }

        public EcoliVectorData ToVectorData()
        {
            Double[] thisArray = { mcg, gvh, lip, chg, aac, alm1, alm2 };
            return new EcoliVectorData(Class, thisArray);
        }
    }
}