using System;
using System.Collections.Generic;
using System.Linq;

namespace Bayes
{
    public class BayesClassification
    {
        private readonly Dictionary<NGramma<int>, WordEntry> _dataSet = new Dictionary<NGramma<int>, WordEntry>();
        private int _totalSpamCount;
        private int _totalNormalCount;
        private int _totalNGrammCount;
        private int _totalMessageCount;
        private readonly Double _spamError;
        private readonly Double _normalError;

        public BayesClassification(Double spamError, Double normalError)
        {
            _spamError = spamError;
            _normalError = normalError;
        }

        public void AddMessageToDataSet(IEnumerable<Message> messages)
        {
            foreach (Message message in messages)
                AddMessageToDataSet(message);
        }
        public void AddMessageToDataSet(Message message)
        {
            _totalMessageCount++;
            if (message.IsSpam)
                _totalSpamCount++;
            else
                _totalNormalCount++;

            foreach (var ngramm in message.NGramms)
            {
                if(!_dataSet.ContainsKey(ngramm))
                    _dataSet.Add(ngramm, new WordEntry());

                _dataSet[ngramm].Increment(message.IsSpam);
                _totalNGrammCount++;
            }
        }

        public int ClassifyMessage(Message message, Double alpha)
        {
            Double spamWeight = Math.Log(_normalError * _totalSpamCount / _totalMessageCount);
            Double normalWeight = Math.Log(_spamError * _totalNormalCount / _totalMessageCount);
            foreach (var ngramm in message.NGramms)
            {
                var entry = _dataSet.GetValueOrDefault(ngramm, WordEntry.EmptyEntry);
                normalWeight += Math.Log((entry.NormalCount + alpha) / (_totalNormalCount + alpha * _totalNGrammCount)) / message.NGramms.Length;
                spamWeight += Math.Log((entry.SpamCount + alpha) / (_totalSpamCount + alpha * _totalNGrammCount)) / message.NGramms.Length;
            }

            return normalWeight > spamWeight ? 0 : 1;
        }
    }
}