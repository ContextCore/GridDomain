using System;
using System.Reflection;

namespace GridDomain.Common
{
    public class MessageMetadataEnvelop : IMessageMetadataEnvelop
    {
        protected MessageMetadataEnvelop(object message, IMessageMetadata metadata)
        {
            Message = message;
            Metadata = metadata;
        }

        public object Message { get; }
        public IMessageMetadata Metadata { get; }

        public static MessageMetadataEnvelop New<T>(T msg, IMessageMetadata metadata = null)
        {
            return new MessageMetadataEnvelop<T>(msg, metadata ?? MessageMetadata.Empty);
        }

        public static IMessageMetadataEnvelop New(object msg, IMessageMetadata metadata)
        {
            var msgType = msg.GetType();
            var constructor = GetEnvelopType(msgType).GetConstructor(new []{msgType, typeof(IMessageMetadata)});
            return (IMessageMetadataEnvelop)constructor.Invoke(new []{msg, metadata});
        }

        public static Type GetEnvelopType(Type type)
        {
            return typeof(MessageMetadataEnvelop<>).MakeGenericType(type);
        }
    }

    public class MessageMetadataEnvelop<T> : MessageMetadataEnvelop,
                                             IMessageMetadataEnvelop<T>
    {
        public MessageMetadataEnvelop(T message, IMessageMetadata metadata) : base(message, metadata) {}

        public new T Message => (T) base.Message;

     
    }
}