using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateHandlerRoute: IEquatable<CreateHandlerRoute>
    {
        public Type MessageType { get; }
        public Type HandlerType { get; }
        /// <summary>
        /// Name of property in message to use as correlation id.
        /// Property must be Guid type. 
        /// All messages with same correlation id will be processed sequencially
        /// to avoid race conditions or concurrency problems.
        /// Can be null.   
        /// </summary>
        public string MessageCorrelationProperty { get; }

        public CreateHandlerRoute(Type messageType, Type handlerType, string messageCorrelationProperty)
        {
            MessageType = messageType;
            HandlerType = handlerType;
            MessageCorrelationProperty = messageCorrelationProperty;

            Check();
        }

        public static CreateHandlerRoute New<TMessage, THandler>(string property) where THandler: IHandler<TMessage>
        {
            return new CreateHandlerRoute(typeof(TMessage), typeof(THandler), property);
        }
        private void Check()
        {
            CheckHandler();
            CheckCorrelationProperty();
        }

        private void CheckHandler()
        {
            var handlerType = typeof (IHandler<>).MakeGenericType(MessageType);
            if (!handlerType.IsAssignableFrom(HandlerType))
                throw new InvalidHandlerType(HandlerType, MessageType);
        }

        private void CheckCorrelationProperty()
        {
            if (MessageCorrelationProperty == null) return;

            var property = MessageType.GetProperty(MessageCorrelationProperty);
            if (property == null)
                throw new CannotFindCorrelationProperty(MessageType, MessageCorrelationProperty);
            if (property.PropertyType != typeof (Guid))
                throw new IncorrectTypeOfCorrelationProperty(MessageType, MessageCorrelationProperty);
        }

        public bool Equals(CreateHandlerRoute other)
        {
            return other.HandlerType == HandlerType && other.MessageType == MessageType;
        }
    }
}