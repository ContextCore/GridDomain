using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class MessageFault: IMessageFault
    {
        public MessageFault(Guid id, object message, Exception ex,Guid sagaId, DateTime occuredTime)
        {
            Id = id;
            Message = message;
            Exception = ex;
            OccuredTime = occuredTime;
        }
        public Guid Id { get; }
        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }
        public object Message { get; }
    }

    public class MessageFault<T> : MessageFault, IMessageFault<T>
    {
        public new T Message { get; }

        public MessageFault(Guid id, T msg, Exception ex, Guid sagaId,  DateTime occuredTime)
            :base(id,msg,ex,sagaId,occuredTime)
        {
            Message = msg;
        }

        public MessageFault(Guid id, T msg, Exception ex) : this(id, msg, ex, Guid.Empty, BusinessDateTime.UtcNow)
        {
            
        }
    }
}