using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;

namespace GridDomain.Tests.Unit.AggregateLifetime {
    class AggregatePersistedHubInfrastructure : IPersistentActorTestsInfrastructure
    {
        public AggregatePersistedHubInfrastructure()
        {
            ChildId = Guid.NewGuid().ToString();
            ChildCreateMessage = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(42, ChildId),
                                                                      MessageMetadata.New(ChildId, null, null));
            ChildActivateMessage = new MessageMetadataEnvelop<ICommand>(new WriteTitleCommand(100, ChildId),
                                                                        MessageMetadata.New(ChildId, null, null));
        }

        Props IPersistentActorTestsInfrastructure.CreateHubProps(ActorSystem system)
        {
            return system.DI().Props<AggregateHubActor<Balloon>>();
        }

        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public string ChildId { get; }
    }
}