using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    class AggregatePersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public IActorRef Hub { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }

        public AggregatePersistedHub_Infrastructure(ActorSystem system)
        {
            ChildId = Guid.NewGuid();
            ChildCreateMessage = new CreateAggregateCommand(42, ChildId, ChildId);
            ChildActivateMessage = new ChangeAggregateCommand(100, ChildId);
            var hubProps = system.DI().Props<TestAggregateHub>();
            Hub = system.ActorOf(hubProps, $"TestHub_{ChildId}");
        }
    }
}