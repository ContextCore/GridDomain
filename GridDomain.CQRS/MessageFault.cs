using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class MessageFault: IMessageFault
    {
        public MessageFault(object message, Exception ex, Type processor, Guid sagaId, DateTime occuredTime)
        {
            Message = message;
            Exception = ex;
            OccuredTime = occuredTime;
            SagaId = sagaId;
            Processor = processor;
        }
        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }
        public object Message { get; }
        public Type Processor { get; }

        public static object CreateGenericFor(object msg, Exception ex, Type processorType)
        {
            var msgType = msg.GetType();
            var methodOpenType = typeof(MessageFault).GetMethod(nameof(New));
            var method = methodOpenType.MakeGenericMethod(msgType);
            return method.Invoke(null, new [] { msg, ex, processorType});
        }

        public static MessageFault<T> New<T>(T msg, Exception ex, Type processorType = null)
        {
            return new MessageFault<T>(msg, ex, processorType,Guid.Empty, BusinessDateTime.UtcNow);
        }
    }

    public class MessageFault<T> : MessageFault, IMessageFault<T>
    {
        public new T Message { get; }

        public MessageFault(T msg, Exception ex, Type processorType, Guid sagaId, DateTime occuredTime)
            : base(msg, ex, processorType, sagaId, occuredTime)
        {
            Message = msg;
        }
    }
}