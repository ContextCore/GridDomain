using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;

namespace GridDomain.Tests.Unit.CommandPipe
{
    public class CommandExecutorActorTests : TestKit
    {
        private class CreateCommand : InflateNewBallonCommand
        {
            public CreateCommand(int title, Guid aggregateId) : base(title, aggregateId) {}
        }

        [Fact]
        public void CommandExecutor_does_not_support_command_inheritance()
        {
            var catalog = new TypeCatalog<Processor, ICommand>();
            catalog.Add<InflateNewBallonCommand>(new Processor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new AggregatesPipeActor(catalog)));

            var msg = new MessageMetadataEnvelop<CreateCommand>(new CreateCommand(1, Guid.NewGuid()), MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectNoMsg();
        }

        [Fact]
        public void CommandExecutor_routes_command_by_its_type()
        {
            var catalog = new TypeCatalog<Processor, ICommand>();
            catalog.Add<InflateNewBallonCommand>(new Processor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new AggregatesPipeActor(catalog)));

            var msg = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(1, Guid.NewGuid()),
                                                           MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectMsg<MessageMetadataEnvelop<ICommand>>();
        }
    }
}