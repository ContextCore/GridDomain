using System;
using Akka.Actor;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    public interface IPersistentActorTestsInfrastructure
    {
        Props HubProps { get; }
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
    }
}