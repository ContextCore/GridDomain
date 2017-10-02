using System;

namespace GridDomain.Transport
{
    public class CannotGetDistributedPubSubException : Exception
    {
        public CannotGetDistributedPubSubException(Exception ex) : base("", ex) {}

        public CannotGetDistributedPubSubException() {}
    }
}