using System;

namespace GridDomain.CQRS.Messaging
{
    public class CannotGetDistributedPubSubException : Exception
    {
        public CannotGetDistributedPubSubException(Exception ex) : base("", ex)
        {
        }

        public CannotGetDistributedPubSubException()
        {
        }
    }
}