using System;
using Akka.Actor;

namespace GridDomain.Tests.XUnit.Aggregate_Sagas_actor_lifetime
{
    public interface IPersistentActorTestsInfrastructure
    {
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
        Props CreateHubProps(ActorSystem system);
    }
}