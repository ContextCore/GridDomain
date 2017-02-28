using System;

namespace GridDomain.CQRS.Messaging.Akka
{
    public class CannotGetDistributedPubSubException : Exception
    {
        public CannotGetDistributedPubSubException(Exception ex) : base("", ex) {}

        public CannotGetDistributedPubSubException() {}
    }
}