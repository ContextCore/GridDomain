using System;
using Akka.Actor;

namespace GridDomain.Tests.XUnit.Aggregate_Sagas_actor_lifetime
{
    public interface IPersistentActorTestsInfrastructure
    {
        Props CreateHubProps(ActorSystem system);
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
    }
}