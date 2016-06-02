using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    internal class InvalidHandlerType : Exception
    {
        public InvalidHandlerType(Type handlerType, Type messageType) :
            base($"Handler {handlerType} should implement {typeof (IHandler<>).MakeGenericType(messageType)}")
        {
            HandlerType = handlerType;
            MessageType = messageType;
        }

        public Type HandlerType { get; set; }
        public Type MessageType { get; set; }
    }
}