using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Aggregate_Sagas_actor_lifetime
{
    public class AggregateHub_children_lifetime_tests : PersistentHubChildrenLifetimeTest
    {
        public AggregateHub_children_lifetime_tests(ITestOutputHelper output) :
            base(output, new PersistentHubFixture(new AggregatePersistedHubInfrastructure())) {}

        class AggregatePersistedHubInfrastructure : IPersistentActorTestsInfrastructure
        {
            Props IPersistentActorTestsInfrastructure.CreateHubProps(ActorSystem system)
            {
                return system.DI().Props<AggregateHubActor<SampleAggregate>>();
            }

            public object ChildCreateMessage { get; }
            public object ChildActivateMessage { get; }
            public Guid ChildId { get; }

            public AggregatePersistedHubInfrastructure()
            {
                ChildId = Guid.NewGuid();
                ChildCreateMessage = new MessageMetadataEnvelop<ICommand>(new CreateSampleAggregateCommand(42, ChildId), new MessageMetadata(ChildId));
                ChildActivateMessage = new MessageMetadataEnvelop<ICommand>(new ChangeSampleAggregateCommand(100, ChildId), new MessageMetadata(ChildId));
            }
        }
    }
}