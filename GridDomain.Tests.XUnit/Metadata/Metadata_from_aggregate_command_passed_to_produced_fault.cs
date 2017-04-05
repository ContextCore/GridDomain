using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using GridDomain.Tests.XUnit.BalloonDomain.Events;
using GridDomain.Tests.XUnit.CommandsExecution;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Metadata
{
    public class Metadata_from_aggregate_command_passed_to_produced_fault : SampleDomainCommandExecutionTests
    {
        public Metadata_from_aggregate_command_passed_to_produced_fault(ITestOutputHelper output) : base(output) {}

        private IMessageMetadataEnvelop<IFault<PlanBallonBlowCommand>> _answer;
        private PlanBallonBlowCommand _command;
        private MessageMetadata _commandMetadata;

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new PlanBallonBlowCommand(Guid.NewGuid(), TimeSpan.FromMilliseconds(50));
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res =
                await Node.Prepare(_command, _commandMetadata).Expect<BalloonCreated>().Execute(null, false);

            _answer = res.Message<IMessageMetadataEnvelop<IFault<PlanBallonBlowCommand>>>();
            // Result_contains_metadata()
            Assert.NotNull(_answer.Metadata);
            // Result_contains_message()
            Assert.NotNull(_answer.Message);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<IFault<PlanBallonBlowCommand>>(_answer.Message);
            //Result_message_has_expected_id()
            Assert.Equal(_command.Id, _answer.Message.Message.Id);
            //Result_message_has_expected_value()
            Assert.Equal(_command.GetType(), _answer.Message.Message.GetType());
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(_command.Id, _answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(_commandMetadata.CorrelationId, _answer.Metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            Assert.Equal(1, _answer.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = _answer.Metadata.History.Steps.First();

            Assert.Equal(AggregateActorName.New<Balloon>(_command.AggregateId).Name, step.Who);
            Assert.Equal(AggregateActor<Balloon>.CommandRaisedAnError, step.Why);
            Assert.Equal(AggregateActor<Balloon>.CreatedFault, step.What);
        }
    }
}