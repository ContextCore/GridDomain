using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateHandlerRouteMessage : IEquatable<CreateHandlerRouteMessage>
    {
        public CreateHandlerRouteMessage(Type messageType, Type handlerType, string messageCorrelationProperty)
        {
            MessageType = messageType;
            HandlerType = handlerType;
            MessageCorrelationProperty = messageCorrelationProperty;

            Check();
        }

        public Type MessageType { get; }
        public Type HandlerType { get; }


        /// <summary>
        ///     Name of property in message to use as correlation id.
        ///     Property must be Guid type.
        ///     All messages with same correlation id will be processed sequencially
        ///     to avoid race conditions or concurrency problems.
        ///     Can be null.
        /// </summary>
        public string MessageCorrelationProperty { get; }

        public bool Equals(CreateHandlerRouteMessage other)
        {
            return other.HandlerType == HandlerType && other.MessageType == MessageType;
        }

        public static CreateHandlerRouteMessage New<TMessage, THandler>(string property) where THandler : IHandler<TMessage>
        {
            return new CreateHandlerRouteMessage(typeof (TMessage), 
                typeof (THandler), 
                property);
        }

        private void Check()
        {
            CheckHandler();
            CheckCorrelationProperty();
        }

        private void CheckHandler()
        {
            var msgType = GetTypeWithoutMetadata(MessageType);
            var handlerType = typeof(IHandler<>).MakeGenericType(msgType);
            if (!handlerType.IsAssignableFrom(HandlerType))
                 throw new InvalidHandlerType(HandlerType, MessageType);
        }

        public static Type GetTypeWithoutMetadata(Type messageType)
        {
            if (typeof(IMessageMetadataEnvelop).IsAssignableFrom(messageType) &&
                messageType.IsGenericType &&
                (messageType.GetGenericTypeDefinition() == typeof(IMessageMetadataEnvelop<>) ||
                 messageType.GetGenericTypeDefinition() == typeof(MessageMetadataEnvelop<>)))
            {
                return messageType.GetGenericArguments().First();
            }

            return messageType;
        }

        private void CheckCorrelationProperty()
        {
            if (MessageCorrelationProperty == null) return;

            var messageType = GetTypeWithoutMetadata(MessageType);

            var property = messageType.GetProperty(MessageCorrelationProperty);
            if (property == null)
                throw new CannotFindCorrelationProperty(messageType, MessageCorrelationProperty);
            if (property.PropertyType != typeof (Guid))
                throw new IncorrectTypeOfCorrelationProperty(messageType, MessageCorrelationProperty);
        }
    }
}