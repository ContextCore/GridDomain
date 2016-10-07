using System;
using GridDomain.Common;

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

    public class Fault: IFault
    {
        public Fault(object message, Exception ex, Type processor, Guid sagaId, DateTime occuredTime)
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

        public static object NewGeneric(object msg, Exception ex, Type processorType, Guid sagaId)
        {
            var msgType = msg.GetType();
            var methodOpenType = typeof(Fault).GetMethod(nameof(New));
            var method = methodOpenType.MakeGenericMethod(msgType);
            return method.Invoke(null, new [] { msg, ex, sagaId, processorType});
        }

        public static Fault<T> New<T>(T msg, Exception ex, Guid sagaId, Type processorType = null)
        {
            return new Fault<T>(msg, ex, processorType, sagaId, BusinessDateTime.UtcNow);
        }
    }
}