using System;
using Akka.Actor;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    public interface IPersistentActorTestsInfrastructure
    {
        IActorRef Hub { get; }
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
    }
}