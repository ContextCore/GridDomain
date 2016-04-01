using System;
using System.Collections.Concurrent;
using System.Linq;

namespace GridDomain.Domain.Tests.ProcessingWait
{
    class ManyUniqueMessagesOfType<T> : IStopCondition
    {
        private readonly int _messagesNumber;
        private readonly MessageOfType<T> _typeCondition = new MessageOfType<T>();
        private readonly ConcurrentBag<object> _knownMessages;
        private readonly Func<object, object, bool> _comparer;

        public ManyUniqueMessagesOfType(Func<T, T, bool> comparer, int messagesNumber)
        {
            _comparer = (m, ms) => comparer((T)m, (T)ms);

            _messagesNumber = messagesNumber;
            _knownMessages = new ConcurrentBag<object>();
        }

        public bool IsMeet(object msg)
        {
            if(! _typeCondition.IsMeet(msg)) return false;

            if(!_knownMessages.Any(m => _comparer(m,msg)))
                _knownMessages.Add(msg);

            return _knownMessages.Count >= _messagesNumber;
        }
    }
}