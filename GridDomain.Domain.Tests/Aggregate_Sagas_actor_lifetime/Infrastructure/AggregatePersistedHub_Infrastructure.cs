using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    class AggregatePersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }

        public AggregatePersistedHub_Infrastructure(ActorSystem system)
        {
            ChildId = Guid.NewGuid();
            ChildCreateMessage = new CreateSampleAggregateCommand(42, ChildId, ChildId);
            ChildActivateMessage = new ChangeSampleAggregateCommand(100, ChildId);
            HubProps = system.DI().Props<TestAggregateHub>();
        }
    }
}