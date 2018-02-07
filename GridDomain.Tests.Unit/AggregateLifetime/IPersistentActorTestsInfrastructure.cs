using System;
using Akka.Actor;

namespace GridDomain.Tests.Unit.AggregateLifetime
{
    public interface IPersistentActorTestsInfrastructure
    {
        object ChildCreateMessage { get; }
        object ChildActivateMessage { get; }
        Guid ChildId { get; }
        Props CreateHubProps(ActorSystem system);
    }
}