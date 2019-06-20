using System;

namespace GridDomain.EventHandlers
{

    public class Sequenced<T>
    {
        public T Message { get; }
        public long Sequence { get; }

        public Sequenced(T message, long sequence)
        {
            Message = message;
            Sequence = sequence;
        }
    }
}