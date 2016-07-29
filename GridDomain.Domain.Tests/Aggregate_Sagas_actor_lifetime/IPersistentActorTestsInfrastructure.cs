using System;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    public interface IPersistentActorTestsInfrastructure
    {
        IActorRef Hub { get; }
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
    }
}