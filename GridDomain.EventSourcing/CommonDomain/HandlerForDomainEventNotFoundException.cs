using System;
using System.Runtime.Serialization;

namespace GridDomain.EventSourcing.CommonDomain
{
    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException(string message)
            : base(message) {}
    }
}