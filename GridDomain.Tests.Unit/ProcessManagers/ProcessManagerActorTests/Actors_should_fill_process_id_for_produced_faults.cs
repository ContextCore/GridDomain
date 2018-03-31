using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;

using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class Actors_should_fill_process_id_for_produced_faults : TestKit
    {

        public Actors_should_fill_process_id_for_produced_faults(ITestOutputHelper output):base("",output)
        {
            
        }

        [Theory]
        [InlineData("test")] //, Description = "unplanned exception from message processor")]
        [InlineData("10")] //, Description = "planned exception from message processor")]
        public async Task Message_process_actor_produce_fault_with_processId_from_incoming_message(string payload)
        {
            var message = new BalloonTitleChanged(payload, Guid.NewGuid().ToString(), DateTime.Now, Guid.NewGuid().ToString());

            var transport = new LocalAkkaEventBusTransport(Sys);
            await transport.Subscribe<IMessageMetadataEnvelop>(TestActor);

            var actor =
                Sys.ActorOf(Props.Create(
                                         () =>
                                             new MessageHandleActor<BalloonTitleChanged, BalloonTitleChangedOddFaultyMessageHandler>(
                                                                                                                   new BalloonTitleChangedOddFaultyMessageHandler(transport),
                                                                                                                   transport)));

            actor.Tell(new MessageMetadataEnvelop<DomainEvent>(message, MessageMetadata.Empty));

            var fault = FishForMessage<IMessageMetadataEnvelop>(m => m.Message is IFault).Message as IFault;

            Assert.Equal(message.ProcessId, fault.ProcessId);
            Assert.IsAssignableFrom<Fault<BalloonTitleChanged>>(fault);
        }

        [Fact]
        public async Task Aggregate_actor_produce_fault_with_processId_from_command()
        {
            var command = new GoSleepCommand(null, null).CloneForProcess(Guid.NewGuid().ToString());

            var transport = Sys.InitLocalTransportExtension().Transport;
            await transport.Subscribe<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(TestActor);
            var handlersActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new HandlersDefaultProcessor(), TestActor)));

            var actor = Sys.ActorOf(Props.Create(() => new AggregateActor<ProgrammerAggregate>(CommandAggregateHandler.New<ProgrammerAggregate>(null),
                                                                                         new SnapshotsPersistencePolicy(1, null,5 , null),
                                                                                         AggregateFactory.Default,
                                                                                         AggregateFactory.Default,
                                                                                         handlersActor)),
                            EntityActorName.New<ProgrammerAggregate>(command.Id).Name);

            actor.Tell(new MessageMetadataEnvelop<ICommand>(command, MessageMetadata.New(command.Id, null, null)));

            var fault = FishForMessage<MessageMetadataEnvelop>(m => m.Message is Fault<GoSleepCommand>).Message as IFault;

            Assert.Equal(command.ProcessId, fault.ProcessId);
        }
    }
}