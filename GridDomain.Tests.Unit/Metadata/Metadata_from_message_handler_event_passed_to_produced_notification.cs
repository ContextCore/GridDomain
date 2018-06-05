using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_message_handler_event_passed_to_produced_notification : NodeTestKit
    {
        public Metadata_from_message_handler_event_passed_to_produced_notification(ITestOutputHelper output) : this(new NodeTestFixture(output).Add(new BalloonDomainConfiguration())) {}
        protected Metadata_from_message_handler_event_passed_to_produced_notification(NodeTestFixture fixture) : base(fixture) {}

        private IMessageMetadataEnvelop<BalloonCreatedNotification> _answer;
        private InflateNewBallonCommand _command;
        private MessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<BalloonCreated> _aggregateEvent;

        [Fact]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            _command = new InflateNewBallonCommand(1, Guid.NewGuid().ToString());
            _commandMetadata = MessageMetadata.New(_command.Id, Guid.NewGuid().ToString(), null);

            var res = await Node.Prepare(_command, _commandMetadata)
                                .Expect<BalloonCreated>()
                                .And<BalloonCreatedNotification>()
                                .Execute();

            _answer = res.MessageWithMetadata<BalloonCreatedNotification>();
            _aggregateEvent = res.MessageWithMetadata<BalloonCreated>();
            //Result_contains_metadata()
            Assert.NotNull(_answer.Metadata);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<BalloonCreatedNotification>(_answer.Message);
            //Result_message_has_expected_id()
            Assert.Equal(_command.AggregateId, _answer.Message.BallonId);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(_aggregateEvent.Metadata.MessageId, _answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(_commandMetadata.CorrelationId, _answer.Metadata.CorrelationId);
            // Result_metadata_has_processed_history_filled_from_aggregate()
            //Assert.Equal(1, _answer.Metadata.History?.Steps.Count);
            ////Result_metadata_has_processed_correct_filled_history_step()
            //var step = _answer.Metadata.History.Steps.First();
            //Assert.Equal(nameof(BalloonCreatedNotificator), step.Who);
            //Assert.Equal(BalloonCreatedNotificator.Why, step.Why);
            //Assert.Equal(BalloonCreatedNotificator.MessageProcessed, step.What);
        }
    }
}