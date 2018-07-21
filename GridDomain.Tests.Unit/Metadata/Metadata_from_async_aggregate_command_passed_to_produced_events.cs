using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
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
    public class Metadata_from_async_aggregate_command_passed_to_produced_events : NodeTestKit
    {
        public Metadata_from_async_aggregate_command_passed_to_produced_events(ITestOutputHelper output) : this(new NodeTestFixture(output)) { }
        protected Metadata_from_async_aggregate_command_passed_to_produced_events(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) { }


        [Fact]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            var command = new PlanTitleChangeCommand(Guid.NewGuid().ToString(),1);
            var commandMetadata = MessageMetadata.New(command.Id, Guid.NewGuid().ToString());

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<BalloonTitleChanged>()
                                .Execute();

            var evt = res.Received;
            var meta = res.ReceivedMetadata;

            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<BalloonTitleChanged>(evt);
            //Result_message_has_expected_id()
            Assert.Equal(command.AggregateId, evt.SourceId);
            //Result_message_has_expected_value()
            Assert.Equal(command.Parameter.ToString(), evt.Value);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(command.Id, meta.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, meta.CorrelationId);
        }
    }
}