using System;

namespace GridDomain.CQRS
{
    public class ExpectedFault<T> : ExpectedFault
    {
        public ExpectedFault(int messageCount, string idPropertyName = null, Guid messageId = new Guid(), params Type[] sources) :
            base(typeof(IFault<T>), typeof(T), messageCount, idPropertyName, messageId, sources)
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

        protected ExpectedFault(Type faultType,
            Type messageType, 
            int messageCount, 
            string idPropertyName = null, 
            Guid messageId = new Guid(), 
            params Type[] sources):
            base(faultType, messageCount, idPropertyName, messageId, sources)
        {
            ProcessMessageType = messageType;
        }

        public static ExpectedFault New(Type messageType, 
                                        string idPropertyName = null, 
                                        Guid messageId = new Guid(),
                                        params Type[] source)
        {
            var faultType = typeof(IFault<>).MakeGenericType(messageType);
            var expect = new ExpectedFault(faultType, messageType,1,idPropertyName,messageId,source);
            ExpectedMessage.VerifyIdPropertyName(expect.ProcessMessageType, expect.IdPropertyName);
            return expect;
        }
    }
}