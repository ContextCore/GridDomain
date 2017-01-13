using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroupMessageProcessException : Exception
    {
        public Type HandlerType { get; set; }

        public ProjectionGroupMessageProcessException(Type handlerType, Exception error):base($"Error occured while processing message in {handlerType}",error)
        {
            HandlerType = handlerType;
        }
    }
}