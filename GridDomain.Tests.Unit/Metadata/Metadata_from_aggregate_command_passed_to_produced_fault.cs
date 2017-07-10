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
    public class Metadata_from_aggregate_command_passed_to_produced_fault : BalloonDomainCommandExecutionTests
    {
        public Metadata_from_aggregate_command_passed_to_produced_fault(ITestOutputHelper output) : base(output) {}

        private PlanBallonBlowCommand _command;
        private MessageMetadata _commandMetadata;

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new PlanBallonBlowCommand(Guid.NewGuid(), TimeSpan.FromMilliseconds(50));
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await Node.Prepare(_command, _commandMetadata)
                                .Expect<BalloonCreated>()
                                .Execute(false);

            // Result_contains_metadata()
            var metadata = res.ReceivedMetadata;
            Assert.NotNull(metadata);
            // Result_contains_message()
            var fault = res.Fault;

            Assert.NotNull(fault);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<IFault<PlanBallonBlowCommand>>(fault);

            //Result_message_has_expected_id()
            Assert.Equal(_command.Id, ((IFault<PlanBallonBlowCommand>)fault).Message.Id);
            //Result_message_has_expected_value()
            Assert.Equal(_command.GetType(), fault.Message.GetType());
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(_command.Id, metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(_commandMetadata.CorrelationId, metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            Assert.Equal(1, metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = metadata.History.Steps.First();

            Assert.Equal(AggregateActorName.New<Balloon>(_command.AggregateId).Name, step.Who);
            Assert.Equal(AggregateActorConstants.CommandRaisedAnError, step.Why);
            Assert.Equal(AggregateActorConstants.CreatedFault, step.What);
        }
    }
}