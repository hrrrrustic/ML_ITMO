using System;
using Microsoft.ML.Data;

namespace NonparametricRegression
{
    public class SeedData
    {
        [LoadColumn(0)] public Single V1 { get; set; }
        [LoadColumn(1)] public Single V2 { get; set; }
        [LoadColumn(2)] public Single V3 { get; set; }
        [LoadColumn(3)] public Single V4 { get; set; }
        [LoadColumn(4)] public Single V5 { get; set; }
        [LoadColumn(5)] public Single V6 { get; set; }
        [LoadColumn(6)] public Single V7 { get; set; }
        [LoadColumn(7)] public Int32 Class { get; set; }

        public override String ToString() => $"{V1} {V2} {V3} {V4} {V5} {V6} {V7} {Class}";
    }
}