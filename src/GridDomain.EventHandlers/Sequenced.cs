using System;

namespace GridDomain.EventHandlers
{

    public class Sequenced
    {
        public object Message { get; }
        public long Sequence { get; }

        public Sequenced(object message, long sequence)
        {
            Message = message;
            Sequence = sequence;
        }
    }
    
    public class Sequenced<T>:Sequenced
    {
        public new T Message => (T) base.Message;

        public Sequenced(T message, long sequence):base(message, sequence)
        {
        }
    }
}