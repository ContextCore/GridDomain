using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class ExpectedFault<T> : ExpectedFault
    {
        public ExpectedFault(int messageCount, string idPropertyName = null, Guid messageId = new Guid(), Type source = null) :
            base(typeof(IMessageFault<T>), typeof(T), messageCount, idPropertyName, messageId, source)
        {
        }
    }

    public class ExpectedFault: ExpectedMessage
    {
        /// <summary>
        /// Contains generic parameter type T of IMessageFault<T>
        /// </summary>
        public Type ProcessMessageType { get; }

        protected override Guid GetMessageId(object msg)
        {
            IMessageFault fault = msg as IMessageFault;
            return fault == null ? base.GetMessageId(msg) : base.GetMessageId(fault.Message);
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
            var faultType = typeof(IMessageFault<>).MakeGenericType(messageType);
            var expect = new ExpectedFault(faultType, messageType,1,idPropertyName,messageId,source);
            ExpectedMessage.VerifyIdPropertyName(expect.ProcessMessageType, expect.IdPropertyName);
            return expect;
        }
    }
}