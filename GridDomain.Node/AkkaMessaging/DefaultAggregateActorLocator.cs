using System;
using CommonDomain.Core;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging
{
    public class DefaultAggregateActorLocator : IAggregateActorLocator
    {
        public Type GetActorTypeFor<T>() where T : AggregateBase
        {
            return typeof (AggregateActor<T>);
        }
    }
}