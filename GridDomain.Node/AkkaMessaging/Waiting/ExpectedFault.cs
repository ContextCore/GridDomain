using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExpectedFault<T> : ExpectedFault
    {
        public ExpectedFault(int messageCount, string idPropertyName = null, Guid messageId = new Guid(), Type source = null) :
            base(typeof(IFault<T>), typeof(T), messageCount, idPropertyName, messageId, source)
        {
        }
    }

    public class ExpectedFault: ExpectedMessage
    {
        /// <summary>
        /// Contains generic parameter type T of IMessageFault<T>
        /// </summary>
        public Type ProcessMessageType { get; }

        protected override bool TryGetMessageId(object msg, out Guid id)
        {
            IFault fault = msg as IFault;
            return fault == null ? base.TryGetMessageId(msg, out id) : base.TryGetMessageId(fault.Message, out id);
        }

        protected ExpectedFault(Type faultType,Type messageType, int messageCount, string idPropertyName = null, Guid messageId = new Guid(), Type source = null):
            base(faultType, messageCount, idPropertyName, messageId, source)
        {
            ProcessMessageType = messageType;
        }

        public static ExpectedFault New(Type messageType, 
                                        string idPropertyName = null, 
                                        Guid messageId = new Guid(),
                                        Type source = null)
        {
            var faultType = typeof(IFault<>).MakeGenericType(messageType);
            var expect = new ExpectedFault(faultType, messageType,1,idPropertyName,messageId,source);
            ExpectedMessage.VerifyIdPropertyName(expect.ProcessMessageType, expect.IdPropertyName);
            return expect;
        }

        //public static object NewGeneric(object msg, Exception ex, Type processorType)
        //{
        //    var msgType = msg.GetType();
        //    var methodOpenType = typeof(MessageFault).GetMethod(nameof(New));
        //    var method = methodOpenType.MakeGenericMethod(msgType);
        //    return method.Invoke(null, new[] { msg, ex, processorType });
        //}
    }
}