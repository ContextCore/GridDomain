using System;

namespace GridDomain.Common
{
    public class MessageMetadataEnvelop : IMessageMetadataEnvelop
    {
        public MessageMetadataEnvelop(object message,  IMessageMetadata metadata)
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

        public static MessageMetadataEnvelop NewGeneric(object msg, IMessageMetadata metadata )
        {
            var msgType = msg.GetType();
            var methodOpenType = typeof(MessageMetadataEnvelop).GetMethod(nameof(New));
            var method = methodOpenType.MakeGenericMethod(msgType);
            return (MessageMetadataEnvelop) method.Invoke(null, new[] { msg, metadata});
        }

        public static Type TypeFor(object msg)
        {
            return typeof(IMessageMetadataEnvelop<>).MakeGenericType(msg.GetType());
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