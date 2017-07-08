using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Saga;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.Sagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_saga_received_event_passed_to_produced_commands : SoftwareProgrammingSagaTest
    {
        public Metadata_from_saga_received_event_passed_to_produced_commands(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public void When_publishing_start_message()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var gotTiredEventMetadata = new MessageMetadata(gotTiredEvent.SourceId,
                                                            BusinessDateTime.UtcNow,
                                                            Guid.NewGuid(),
                                                            Guid.NewGuid());

            Node.Pipe.SagaProcessor.Tell(new Initialize(TestActor));
            Node.Pipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent>(gotTiredEvent,
                                                                                 gotTiredEventMetadata));

            var answer = FishForMessage<MessageMetadataEnvelop<ICommand>>(m => true);
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
            Assert.Equal(1, answer.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = answer.Metadata.History.Steps.First();
            var name = AggregateActorName.New<SoftwareProgrammingState>(command.SagaId);

            Assert.Equal(name.Name, step.Who);
            Assert.Equal(SagaActorLiterals.SagaProducedACommand, step.Why);
            Assert.Equal(SagaActorLiterals.PublishingCommand, step.What);
        }
    }
}