using System;

namespace GridDomain.Node.AkkaMessaging
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