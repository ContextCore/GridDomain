using System;

namespace GridDomain.CQRS
{
    public class Fault<T> : Fault, IFault<T>
    {
        public new T Message { get; }

        public Fault(T msg, Exception ex, Type processorType, Guid sagaId, DateTime occuredTime)
            : base(msg, ex, processorType, sagaId, occuredTime)
        {
            Message = msg;
        }
    }
}