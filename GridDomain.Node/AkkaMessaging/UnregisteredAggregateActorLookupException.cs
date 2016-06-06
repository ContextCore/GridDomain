using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class UnregisteredAggregateActorLookupException : Exception
    {
        public UnregisteredAggregateActorLookupException(Type type) :
            base("Cannot find registered actor for aggregate " + type)
        {
        }
    }
}