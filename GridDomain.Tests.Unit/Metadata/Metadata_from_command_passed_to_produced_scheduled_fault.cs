using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_command_passed_to_produced_scheduled_fault : FutureEventsTest
    {
        public Metadata_from_command_passed_to_produced_scheduled_fault(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            var command = new ScheduleErrorInFutureCommand(DateTime.Now.AddMilliseconds(100), Guid.NewGuid(), "12", 1);
            var commandMetadata = new MessageMetadata(command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<JobFailed>()
                                .And<Fault<RaiseScheduledDomainEventCommand>>()
                                .Execute(null, false);

            var schedulingCommandFault = res.Message<IMessageMetadataEnvelop<IFault<RaiseScheduledDomainEventCommand>>>();
            var jobFailedEnvelop = res.Message<IMessageMetadataEnvelop<JobFailed>>();

            //Result_contains_metadata()
            Assert.NotNull(schedulingCommandFault.Metadata);
            //Result_contains_message()
            Assert.NotNull(schedulingCommandFault.Message);
            //Result_message_has_expected_type()
            Assert.IsAssignableFrom<IFault<RaiseScheduledDomainEventCommand>>(schedulingCommandFault.Message);
            //Result_message_has_expected_id()
            Assert.Equal((jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id,
                         schedulingCommandFault.Message.Message.Id);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal((jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id,
                         schedulingCommandFault.Metadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, schedulingCommandFault.Metadata.CorrelationId);
            //Result_metadata_has_processed_history_filled_from_aggregate()
            Assert.Equal(1, schedulingCommandFault.Metadata.History?.Steps.Count);
            //Result_metadata_has_processed_correct_filled_history_step()
            var step = schedulingCommandFault.Metadata.History.Steps.First();

            Assert.Equal(AggregateActorName.New<TestFutureEventsAggregate>(command.AggregateId).Name, step.Who);
            Assert.Equal(AggregateActorConstants.CommandRaisedAnError, step.Why);
            Assert.Equal(AggregateActorConstants.CreatedFault, step.What);
        }
    }
}