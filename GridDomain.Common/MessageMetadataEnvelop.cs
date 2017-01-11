using System;

namespace GridDomain.Common
{
    public class MessageMetadataEnvelop : IMessageMetadataEnvelop
    {
        protected MessageMetadataEnvelop(object message,  IMessageMetadata metadata)
        {
            Message = message;
            Metadata = metadata;
        }

        public object Message { get; }
        public IMessageMetadata Metadata { get; }

        public static MessageMetadataEnvelop New<T>(T msg, IMessageMetadata metadata)
        {
            return new MessageMetadataEnvelop<T>(msg, metadata);
        }

        public static IMessageMetadataEnvelop NewGeneric(object msg, IMessageMetadata metadata )
        {
            var msgType = msg.GetType();
            var methodOpenType = typeof(MessageMetadataEnvelop).GetMethod(nameof(New));
            var method = methodOpenType.MakeGenericMethod(msgType);
            return (IMessageMetadataEnvelop) method.Invoke(null, new[] { msg, metadata});
        }

        public static Type GenericForMessage(object msg)
        {
            return GenericForType(msg.GetType());
        }
        public static Type GenericForType(Type type)
        {
            return typeof(IMessageMetadataEnvelop<>).MakeGenericType(type);
        }
    }

    public class MessageMetadataEnvelop<T> : MessageMetadataEnvelop, IMessageMetadataEnvelop<T>
    {
        public MessageMetadataEnvelop(T message, IMessageMetadata metadata) : base(message, metadata)
        {
        }

        public new T Message => (T)base.Message;
    }
}