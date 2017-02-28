using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Metadata
{
    public class Metadata_from_message_handler_event_passed_to_produced_notification : SampleDomainCommandExecutionTests
    {
        public Metadata_from_message_handler_event_passed_to_produced_notification(ITestOutputHelper output) : base(output) {}

        private IMessageMetadataEnvelop<AggregateCreatedEventNotification> _answer;
        private CreateSampleAggregateCommand _command;
        private MessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<SampleAggregateCreatedEvent> _aggregateEvent;

        [Fact]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            _command = new CreateSampleAggregateCommand(1, Guid.NewGuid());
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res =
                await
                    Node.Prepare(_command, _commandMetadata)
                        .Expect<SampleAggregateCreatedEvent>()
                        .And<AggregateCreatedEventNotification>()
                        .Execute();

            _answer = res.MessageWithMetadata<AggregateCreatedEventNotification>();
            _aggregateEvent = res.MessageWithMetadata<SampleAggregateCreatedEvent>();
            //Result_contains_metadata()
            Assert.NotNull(_answer.Metadata);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<AggregateCreatedEventNotification>(_answer.Message);
            //Result_message_has_expected_id()
            Assert.Equal(_command.AggregateId, _answer.Message.AggregateId);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(_aggregateEvent.Metadata.MessageId, _answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(_commandMetadata.CorrelationId, _answer.Metadata.CorrelationId);
            // Result_metadata_has_processed_history_filled_from_aggregate()
            Assert.Equal(1, _answer.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = _answer.Metadata.History.Steps.First();
            Assert.Equal(nameof(AggregateCreatedProjectionBuilder), step.Who);
            Assert.Equal(AggregateCreatedProjectionBuilder.Why, step.Why);
            Assert.Equal(AggregateCreatedProjectionBuilder.MessageProcessed, step.What);
        }
    }
}