using System;

namespace GridDomain.Node.Transports
{
    public class CannotGetDistributedPubSubException : Exception
    {
        public CannotGetDistributedPubSubException(Exception ex) : base("", ex) {}

        public CannotGetDistributedPubSubException() {}
    }
}