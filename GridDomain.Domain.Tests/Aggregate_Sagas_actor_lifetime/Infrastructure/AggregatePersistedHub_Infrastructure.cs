using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Tests.SampleDomain;
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
            ChildCreateMessage = new MessageMetadataEnvelop<ICommand>(new CreateSampleAggregateCommand(42, ChildId), new MessageMetadata(ChildId));
            ChildActivateMessage = new MessageMetadataEnvelop<ICommand>(new ChangeSampleAggregateCommand(100, ChildId), new MessageMetadata(ChildId));
            HubProps = system.DI().Props<AggregateHubActor<SampleAggregate>>();
        }
    }
}