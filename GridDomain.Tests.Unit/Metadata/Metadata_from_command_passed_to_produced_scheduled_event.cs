using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_command_passed_to_produced_scheduled_event : FutureEventsTest
    {
        public Metadata_from_command_passed_to_produced_scheduled_event(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            var command = new ScheduleEventInFutureCommand(DateTime.Now.AddMilliseconds(100), Guid.NewGuid(), "12");
            var commandMetadata = new MessageMetadata(command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<TestDomainEvent>()
                                .And<JobSucceeded>()
                                .Execute();

            var answer = res.MessageWithMetadata<TestDomainEvent>();
            var jobSucced = res.MessageWithMetadata<JobSucceeded>();

            //Result_contains_metadata()
            Assert.NotNull(answer.Metadata);
            //Result_contains_message()
            Assert.NotNull(answer.Message);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<TestDomainEvent>(answer.Message);
            //Result_message_has_expected_id()
            Assert.Equal(command.AggregateId, answer.Message.SourceId);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal((jobSucced.Message.Message as ICommand)?.Id, answer.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, answer.Metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            Assert.Equal(1, answer.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = answer.Metadata.History.Steps.First();

            Assert.Equal(AggregateActorName.New<FutureEventsAggregate>(command.AggregateId).Name, step.Who);
            Assert.Equal(SimpleAggregateActorConstants.CommandExecutionCreatedAnEvent, step.Why);
            Assert.Equal(SimpleAggregateActorConstants.PublishingEvent, step.What);
        }
    }
}