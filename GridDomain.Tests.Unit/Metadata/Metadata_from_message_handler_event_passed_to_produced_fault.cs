using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_message_handler_event_passed_to_produced_fault : NodeTestKit
    {
        public Metadata_from_message_handler_event_passed_to_produced_fault(ITestOutputHelper output)
            : this(new NodeTestFixture(output, new []{new FaultyBalloonProjectionDomainConfiguration()})) {}
        
        protected Metadata_from_message_handler_event_passed_to_produced_fault(NodeTestFixture fixture):base(fixture){}

        [Fact]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            var command = new InflateNewBallonCommand(1, Guid.NewGuid().ToString());
            var commandMetadata = MessageMetadata.New(command.Id, Guid.NewGuid().ToString(), null);

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<BalloonCreated>()
                                .And<Fault<BalloonCreated>>()
                                .Execute(null, false);

            var answer = res.MessageWithMetadata<IFault<BalloonCreated>>();
            var aggregateEvent = res.MessageWithMetadata<BalloonCreated>();

            //Result_contains_metadata()
            Assert.NotNull(answer.Metadata);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<IFault<BalloonCreated>>(answer.Message);
            //Produced_fault_produced_by_projection_builder()
            Assert.Equal(typeof(BalloonCreatedFaultyProjection), answer.Message.Processor);
            //Result_message_has_expected_id()
            Assert.Equal(command.AggregateId, answer.Message.Message.SourceId);
            //Result_metadata_has_aggregate_event_id_as_casuation_id()
            Assert.Equal(aggregateEvent.Metadata.MessageId, answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, answer.Metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            //Assert.Equal(1, answer.Metadata.History?.Steps.Count);
            ////Result_metadata_has_processed_correct_filled_history_step()
            //var step = answer.Metadata.History.Steps.First();
            //
            //Assert.Equal(nameof(BalloonCreatedFaultyProjection), step.Who);
            //Assert.Equal(MessageHandlingConstants.MessageProcessCasuedAnError, step.Why);
            //Assert.Equal(MessageHandlingConstants.PublishingFault, step.What);
        }
    }
}