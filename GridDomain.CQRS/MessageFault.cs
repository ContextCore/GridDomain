using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class MessageFault<T> : IMessageFault<T>
    {
        public Guid Id { get; }
        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }
        public T Message { get; }
        object IMessageFault.Message => Message;

        public MessageFault(Guid id, T msg, Exception ex, DateTime occuredTime)
        {
            Id = id;
            Message = msg;
            Exception = ex;
            OccuredTime = occuredTime;
        }
        public MessageFault(Guid id, T msg, Exception ex):this(id,msg,ex,BusinessDateTime.UtcNow)
        {
        }
    }
}