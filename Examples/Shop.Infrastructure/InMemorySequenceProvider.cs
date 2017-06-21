using System.Collections.Generic;

namespace Shop.Infrastructure
{
    /// <summary>
    ///     Naive in-memory implementation for sequence provider
    /// </summary>
    public class InMemorySequenceProvider : ISequenceProvider
    {
        public const string GlobalSequenceName = "global";
        private readonly object _locker = new object();
        private readonly Dictionary<string, long> _sequences = new Dictionary<string, long>();

        public long GetNext(string sequenceName = null)
        {
            lock (_locker)
            {
                sequenceName = sequenceName ?? GlobalSequenceName;
                long nextNumber;
                if (_sequences.TryGetValue(sequenceName, out nextNumber))
                    ++nextNumber;
                else
                    nextNumber = 1;

                _sequences[sequenceName] = nextNumber;
                return nextNumber;
            }
        }
    }
}