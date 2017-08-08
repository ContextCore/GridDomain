using System;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing.CommonDomain
{
    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException() {}

        public HandlerForDomainEventNotFoundException(string message)
            : base(message) {}

        public HandlerForDomainEventNotFoundException(string message, Exception innerException)
            : base(message, innerException) {}

        public HandlerForDomainEventNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}