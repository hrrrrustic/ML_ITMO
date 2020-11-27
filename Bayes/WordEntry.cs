using System;

namespace Bayes
{
    public class WordEntry
    {
        public static readonly WordEntry EmptyEntry = new WordEntry();
        public int SpamCount { get; private set; }
        public int NormalCount { get; private set; }

        public void Increment(bool isSpam)
        {
            if (isSpam)
                SpamCount++;
            else
                NormalCount++;
        }

        public int TotalCount => SpamCount + NormalCount;
    }
}
