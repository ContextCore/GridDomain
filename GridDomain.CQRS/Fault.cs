using System;
using System.Reflection;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public class Fault<T> : Fault,
                            IFault<T>
    {
        public Fault(string id, T message, Exception exception, Type processorType, string processId, DateTime occuredTime)
            : base(id, message, exception, processorType, processId, occuredTime)
        {
            Message = message;
        }

        public new T Message { get; }
    }


    public class Fault : IFault
    {
        private static readonly MethodInfo MethodOpenType = typeof(Fault).GetTypeInfo().GetMethod(nameof(New));

        public Fault(string id, object message, Exception exception, Type processor, string processId, DateTime occuredTime)
        {
            Id = id;
            Message = message;
            Exception = exception;
            OccuredTime = occuredTime;
            ProcessId = processId;
            Processor = processor;
        }

        public Exception Exception { get; }
        public string ProcessId { get; }
        public DateTime OccuredTime { get; }
        public string Id { get; }
        public object Message { get; }
        public Type Processor { get; }

        public static Fault NewGeneric(string id,object msg, Exception exception, string processId, Type processorType)
        {
            var msgType = msg.GetType();
            var method = MethodOpenType.MakeGenericMethod(msgType);
            return (Fault) method.Invoke(null, new[] {id, msg, exception, processId, processorType});
        }

        public static Type TypeFor(object msg)
        {
            return typeof(Fault<>).MakeGenericType(msg.GetType());
        }

        public static Fault<T> New<T>(string id, T msg, Exception ex, string processId, Type processorType = null)
        {
            return new Fault<T>(id, msg, ex, processorType, processId, BusinessDateTime.UtcNow);
        }
    }
}