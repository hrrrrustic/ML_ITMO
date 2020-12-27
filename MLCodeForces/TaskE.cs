using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MLCodeForces
{
    public class Message
    {
        public readonly List<string> Words;
        public readonly int Label;

        public Message(List<string> messageWords, int label)
        {
            Words = messageWords;
            Label = label;
        }
    }

    public class BayesClassification
    {
        private readonly Dictionary<int, Dictionary<string, double>> _dataSet = new Dictionary<int, Dictionary<string, double>>();
        private readonly HashSet<String> _allWords = new HashSet<String>();
    }

    public class TaskE
    {
        public static void Solve()
        {
            /*var classCount = Int32.Parse(Console.ReadLine());
            var classificationPenalty = Console.ReadLine().ReadNumbers().ToArray();
            var smoothing = Int32.Parse(Console.ReadLine());
            var trainCount = Int32.Parse(Console.ReadLine());
            var bayes = new BayesClassification(classificationPenalty, smoothing, classCount);
            for (int i = 0; i < trainCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .Split(' ');

                int label = Int32.Parse(row[0]) - 1;
                int wordCount = Int32.Parse(row[1]);
                List<string> words = new List<String>(wordCount);
                for (int j = 2; j < 2 + wordCount; j++)
                    words.Add(row[j]);

                bayes.AddMessageToDataSet(new Message(words, label));
            }

            var testCount = Int32.Parse(Console.ReadLine());

            for (int i = 0; i < testCount; i++)
            {
                var row = Console
                    .ReadLine()
                    .Split(' ');

                var predict = bayes.ClassifyMessage(row);
                Console.WriteLine(String.Join(' ', predict));
            }*/
        }
    }
}