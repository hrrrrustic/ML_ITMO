using System;
using System.Linq;

namespace Bayes
{
    public class Message
    {
        public readonly NGramma<int>[] NGramms;
        public readonly bool IsSpam;

        public Message(Int32[] messageWords, Boolean isSpam, int n)
        {
            NGramms = NGramma<int>.GetNGrammaFromArray(messageWords, n);
            IsSpam = isSpam;
        }
    }
}