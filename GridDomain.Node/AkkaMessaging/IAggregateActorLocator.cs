using System;
using CommonDomain.Core;

namespace GridDomain.Node.AkkaMessaging
{
    public interface IAggregateActorLocator
    {
        Type GetActorTypeFor<T>() where T : AggregateBase;
    }
}