using System;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class Fault<T> : Fault,
                            IFault<T>
    {
        public Fault(T message, Exception exception, Type processorType, Guid sagaId, DateTime occuredTime)
            : base(message, exception, processorType, sagaId, occuredTime)
        {
            Message = message;
        }

        public new T Message { get; }
    }

    public class Fault : IFault
    {
        public Fault(object message, Exception exception, Type processor, Guid sagaId, DateTime occuredTime)
        {
            Message = message;
            Exception = exception;
            OccuredTime = occuredTime;
            SagaId = sagaId;
            Processor = processor;
        }

        public Exception Exception { get; }
        public Guid SagaId { get; }
        public DateTime OccuredTime { get; }
        public object Message { get; }
        public Type Processor { get; }

        public static Fault NewGeneric(object msg, Exception exception, Guid sagaId, Type processorType)
        {
            var msgType = msg.GetType();
            var methodOpenType = typeof(Fault).GetMethod(nameof(New));
            var method = methodOpenType.MakeGenericMethod(msgType);
            return (Fault) method.Invoke(null, new[] {msg, exception, sagaId, processorType});
        }

        public static Type TypeFor(object msg)
        {
            return typeof(Fault<>).MakeGenericType(msg.GetType());
        }

        public static Fault<T> New<T>(T msg, Exception ex, Guid sagaId, Type processorType = null)
        {
            return new Fault<T>(msg, ex, processorType, sagaId, BusinessDateTime.UtcNow);
        }
    }
}