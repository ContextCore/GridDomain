using System;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateRoute: IEquatable<CreateRoute>
    {
        public Type MessageType { get; }
        public Type HandlerType { get; }
        /// <summary>
        /// Name of property in message to use as correlation id.
        /// Property must be Guid type. 
        /// All messages with same correlation id will be processed sequencially
        /// to avoid race conditions or concurrency problems  
        /// </summary>
        public string MessageCorrelationProperty { get; }

        public CreateRoute(Type messageType, Type handlerType, string messageCorrelationProperty)
        {
            MessageType = messageType;
            HandlerType = handlerType;
            MessageCorrelationProperty = messageCorrelationProperty;

            Check();
        }

        public static CreateRoute New<TMessage, THandler>(string property) where THandler: IHandler<TMessage>
        {
            return new CreateRoute(typeof(TMessage), typeof(THandler), property);
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
                throw new BadRoute.InvalidHandlerType(HandlerType, MessageType);
        }

        private void CheckCorrelationProperty()
        {
            var property = MessageType.GetProperty(MessageCorrelationProperty);
            if (property == null)
                throw new BadRoute.CannotFindCorrelationProperty(MessageType, MessageCorrelationProperty);
            if (property.PropertyType != typeof (Guid))
                throw new BadRoute.IncorrectTypeOfCorrelationProperty(MessageType, MessageCorrelationProperty);
        }

        public bool Equals(CreateRoute other)
        {
            return other.HandlerType == HandlerType && other.MessageType == MessageType;
        }
    }
}

namespace GridDomain.Node.AkkaMessaging.BadRoute
{

    class CannotFindCorrelationProperty : Exception
    {
        public Type Type { get; set; }
        public string Property { get; set; }

        public CannotFindCorrelationProperty(Type type, string property):
            base($"Cannot find property {property} in type {type}")
        {
            Type = type;
            Property = property;
        }
    }

    internal class IncorrectTypeOfCorrelationProperty : Exception
    {
        public Type Type { get; set; }
        public string Property { get; set; }

        public IncorrectTypeOfCorrelationProperty(Type type, string property):
            base($"Correlation property {property} of type {type} should be {typeof(Guid)} type to act as correlation property")
        {
            Type = type;
            Property = property;
        }
    }

    internal class InvalidHandlerType : Exception
    {
        public Type HandlerType { get; set; }
        public Type MessageType { get; set; }

        public InvalidHandlerType(Type handlerType, Type messageType): 
            base($"Handler {handlerType} should implement {typeof(IHandler<>).MakeGenericType(messageType)}")
        {
            HandlerType = handlerType;
            MessageType = messageType;
        }
    }
}