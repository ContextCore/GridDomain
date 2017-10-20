using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Metadata_from_command_passed_to_produced_scheduled_fault : FutureEventsTest
    {
        public Metadata_from_command_passed_to_produced_scheduled_fault(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            var commandMetadata = new MessageMetadata(Guid.NewGuid(), Guid.NewGuid());
            var command = new PlanBoomCommand(Guid.NewGuid(), DateTime.Now.AddMilliseconds(100));

            var res = await Node.Prepare(command, commandMetadata)
                                .Expect<JobFailed>()
                                .And<Fault<RaiseScheduledDomainEventCommand>>()
                                .Execute(null, false);

            var schedulingCommandFault = res.MessageWithMetadata<Fault<RaiseScheduledDomainEventCommand>>();
            var jobFailedEnvelop = res.MessageWithMetadata<JobFailed>();

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
                         Guid.Parse(schedulingCommandFault.Metadata.CasuationId));
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(commandMetadata.CorrelationId, schedulingCommandFault.Metadata.CorrelationId);
        }
    }
}