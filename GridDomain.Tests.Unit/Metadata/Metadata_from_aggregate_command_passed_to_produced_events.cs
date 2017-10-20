using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_aggregate_command_passed_to_produced_events : BalloonDomainCommandExecutionTests
    {
        public Metadata_from_aggregate_command_passed_to_produced_events(ITestOutputHelper output) : base(output) {}
        private InflateNewBallonCommand _command;
        private MessageMetadata _commandMetadata;

        [Fact]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            _command = new InflateNewBallonCommand(1, Guid.NewGuid());
            _commandMetadata = new MessageMetadata(_command.Id, Guid.NewGuid());

            var res = await Node.Prepare(_command, _commandMetadata)
                                .Expect<BalloonCreated>()
                                .Execute();
            
            var messageMetadata = res.ReceivedMetadata;
            var balloonCreated = res.Received;

            Assert.NotNull(messageMetadata);
            //Result_contains_message()

            Assert.NotNull(balloonCreated);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<BalloonCreated>(balloonCreated);
            //Result_message_has_expected_id()
            Assert.Equal(_command.AggregateId, balloonCreated.SourceId);
            //Result_message_has_expected_value()
            Assert.Equal(_command.Title.ToString(), balloonCreated.Value);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(_command.Id, Guid.Parse(messageMetadata.CasuationId));
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(_commandMetadata.CorrelationId, messageMetadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            //Assert.Equal(1, messageMetadata.History?.Steps.Count);
            ////Result_metadata_has_processed_correct_filled_history_step()
            //var step = messageMetadata.History.Steps.First();
            //
            //Assert.Equal(AggregateActorName.New<Balloon>(_command.AggregateId).Name, step.Who);
            //Assert.Equal(AggregateActorConstants.CommandExecutionCreatedAnEvent, step.Why);
            //Assert.Equal(AggregateActorConstants.PublishingEvent, step.What);
        }
    }
}