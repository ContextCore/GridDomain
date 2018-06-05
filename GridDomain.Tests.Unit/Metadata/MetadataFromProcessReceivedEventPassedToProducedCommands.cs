using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class MetadataFromProcessReceivedEventPassedToProducedCommands : NodeTestKit
    {
        public MetadataFromProcessReceivedEventPassedToProducedCommands(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) {}
        protected MetadataFromProcessReceivedEventPassedToProducedCommands(NodeTestFixture fixture):base(fixture){}
        
        [Fact]
        public void When_publishing_start_message()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var gotTiredEventMetadata = MessageMetadata.New(gotTiredEvent.SourceId, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            Node.Pipe.ProcessesPipeActor.Tell(new Initialize(TestActor));
            Node.Pipe.ProcessesPipeActor.Tell(new MessageMetadataEnvelop<DomainEvent>(gotTiredEvent,
                                                                                 gotTiredEventMetadata));

            var answer = FishForMessage<MessageMetadataEnvelop>(m => true,TimeSpan.FromSeconds(5));
            var command = answer.Message as MakeCoffeCommand;

            //Result_contains_metadata()
            Assert.NotNull(answer.Metadata);
            //Result_contains_message()
            Assert.NotNull(answer.Message);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<MakeCoffeCommand>(answer.Message);
            //Result_message_has_expected_value()
            Assert.Equal(gotTiredEvent.PersonId, command.PersonId);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(gotTiredEvent.SourceId, answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(gotTiredEventMetadata.CorrelationId, answer.Metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            //Assert.Equal(1, answer.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            //var step = answer.Metadata.History.Steps.First();
            //var name = AggregateActorName.New<SoftwareProgrammingState>(command.ProcessId);
            //
            //Assert.Equal(name.Name, step.Who);
            //Assert.Equal(ProcessManagerActorConstants.ProcessProducedACommand, step.Why);
            //Assert.Equal(ProcessManagerActorConstants.PublishingCommand, step.What);
        }
    }
}