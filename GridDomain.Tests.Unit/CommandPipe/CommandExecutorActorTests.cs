using System;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
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
            var catalog = new TypeCatalog<IMessageProcessor<CommandExecuted>, ICommand>();
            catalog.Add<InflateNewBallonCommand>(new CommandProcessor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new AggregatesPipeActor(catalog)));

            var msg = new MessageMetadataEnvelop<CreateCommand>(new CreateCommand(1, Guid.NewGuid()), MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectNoMsg();
        }


        [Fact]
        public void CommandExecutor_routes_command_by_its_type()
        {
            var catalog = new TypeCatalog<IMessageProcessor<CommandExecuted>, ICommand>();
            catalog.Add<InflateNewBallonCommand>(new CommandProcessor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new AggregatesPipeActor(catalog)));

            var msg = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(1, Guid.NewGuid()),
                                                           MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectMsg<MessageMetadataEnvelop<ICommand>>();
        }

        [Fact]
        public void CommandExecutor_return_message_on_command_completed()
        {
            var catalog = new TypeCatalog<IMessageProcessor<CommandExecuted>, ICommand>();
            var cfg = new BalloonDomainConfiguration();
            var container = new ContainerBuilder();
            
            Sys.AddDependencyResolver(new AutoFacDependencyResolver(container, System));
            var hub = Sys.ActorOf(Props.Create(() => new AggregateHubActor<Balloon>(new DefaultPersistentChildsRecycleConfiguration())));
            catalog.Add<InflateNewBallonCommand>(new CommandProcessor(hub));

            var actor = Sys.ActorOf(Props.Create(() => new AggregatesPipeActor(catalog)));

            var msg = new MessageMetadataEnvelop<ICommand>(new InflateNewBallonCommand(1, Guid.NewGuid()),
                                                           MessageMetadata.Empty);

            actor.Tell(msg,TestActor);
            ExpectMsg<CommandExecuted>();
        }
    }
}