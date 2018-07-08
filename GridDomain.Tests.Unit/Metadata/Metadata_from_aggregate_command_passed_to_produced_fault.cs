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
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_aggregate_command_passed_to_produced_fault : NodeTestKit
    {
        public Metadata_from_aggregate_command_passed_to_produced_fault(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected Metadata_from_aggregate_command_passed_to_produced_fault(NodeTestFixture output) : base(output.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            var command = new BlowBalloonCommand(Guid.NewGuid().ToString());
            var commandMetadata = MessageMetadata.New(command.Id, Guid.NewGuid().ToString(), null);

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<Fault<BlowBalloonCommand>>()
                                .Execute(false);

            // Result_contains_metadata()
            var metadata = res.ReceivedMetadata;
            Assert.NotNull(metadata);
            // Result_contains_message()
            var fault = res.Received;

            Assert.NotNull(fault);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<IFault<BlowBalloonCommand>>(fault);

            //Result_message_has_expected_id()
            Assert.Equal(command.Id, ((IFault<BlowBalloonCommand>)fault).Message.Id);
            //Result_message_has_expected_value()
            Assert.Equal(command.GetType(), fault.Message.GetType());
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(command.Id, metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            //Assert.Equal(1, metadata.History?.Steps.Count);
            ////Result_metadata_has_processed_correct_filled_history_step()
            //var step = metadata.History.Steps.First();
            //
            //Assert.Equal(AggregateActorName.New<Balloon>(command.AggregateId).Name, step.Who);
            //Assert.Equal(AggregateActorConstants.CommandRaisedAnError, step.Why);
            //Assert.Equal(AggregateActorConstants.CommandExecutionFinished, step.What);
        }
    }
}