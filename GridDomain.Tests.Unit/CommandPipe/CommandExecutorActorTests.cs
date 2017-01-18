using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using Moq;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandPipe
{
    [TestFixture]
    class CommandExecutorActorTests : TestKit
    {
        [Test]
        public void CommandExecutor_routes_command_by_its_type()
        {
            var catalog = new AggregateProcessorCatalog();
            catalog.Add<CreateSampleAggregateCommand>(new Processor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new CommandExecutionActor(catalog)));

            var msg = new MessageMetadataEnvelop<ICommand>(new CreateSampleAggregateCommand(1, Guid.NewGuid()),MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectMsg<MessageMetadataEnvelop<ICommand>>();
        }

        class CreateCommand : CreateSampleAggregateCommand
        {
            public CreateCommand(int parameter, Guid aggregateId) : base(parameter, aggregateId)
            {
            }
        }

        [Test]
        public void CommandExecutor_does_not_support_command_inheritance()
        {
            var catalog = new AggregateProcessorCatalog();
            catalog.Add<CreateSampleAggregateCommand>(new Processor(TestActor));

            var actor = Sys.ActorOf(Props.Create(() => new CommandExecutionActor(catalog)));

            var msg = new MessageMetadataEnvelop<CreateCommand>(new CreateCommand(1, Guid.NewGuid()), MessageMetadata.Empty);

            actor.Tell(msg);

            ExpectNoMsg();
        }
    }
}
