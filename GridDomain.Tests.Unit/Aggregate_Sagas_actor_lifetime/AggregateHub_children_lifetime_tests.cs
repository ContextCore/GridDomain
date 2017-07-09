using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{
    public class AggregateHub_children_lifetime_tests : PersistentHubChildrenLifetimeTest
    {
        public AggregateHub_children_lifetime_tests(ITestOutputHelper output)
            : base(output, new PersistentHubFixture(new AggregatePersistedHubInfrastructure())) {}

        private class AggregatePersistedHubInfrastructure : IPersistentActorTestsInfrastructure
        {
            public AggregatePersistedHubInfrastructure()
            {
                ChildId = Guid.NewGuid();
                ChildCreateMessage = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(42, ChildId),
                                                                          new MessageMetadata(ChildId));
                ChildActivateMessage = new MessageMetadataEnvelop<ICommand>(new WriteTitleCommand(100, ChildId),
                                                                            new MessageMetadata(ChildId));
            }

            Props IPersistentActorTestsInfrastructure.CreateHubProps(ActorSystem system)
            {
                return system.DI().Props<AggregateHubActor<Balloon>>();
            }

            public object ChildCreateMessage { get; }
            public object ChildActivateMessage { get; }
            public Guid ChildId { get; }
        }
    }
}