using System;
using System.Collections.Generic;

namespace Bayes
{
    public class CrossValidation
    {
        public static ConfusionMatrix Validate(List<List<Message>> data, int k, Double smoothingAlpha, Double spamError, Double normalError,
            out List<(int, int)> rocData)
        {
            rocData = new List<(Int32, Int32)>();
            Int32[,] matrix = new Int32[2, 2];

            for (int i = 0; i < k; i++)
            {
                var classifier = new BayesClassification(spamError, normalError);
                for (int j = 0; j < k; j++)
                {
                    if (i == j)
                        continue;

                    IList<Message> pack = data[j];
                    classifier.AddMessageToDataSet(pack);
                }

                foreach (Message item in data[i])
                {
                    Int32 predict = classifier.ClassifyMessage(item, smoothingAlpha);
                    Int32 actual = item.IsSpam ? 1 : 0;
                    matrix[actual, predict]++;
                    rocData.Add((predict, actual));
                }
            }

            return new ConfusionMatrix(matrix);
        }
    }
}