using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node
{
    public static class ActorSystemExtensions
    {
        public static IActorRef CreatePersistedIdentityActor(this ActorSystem system, Type actorType, Guid id)
        {
            var props = system.DI().Props(actorType);
            var actor = system.ActorOf(props, new AggregateActorName(actorType,id).ToString());
            return actor;
        }
    }
}