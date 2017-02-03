using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.XUnit.FutureEvents;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.Metadata
{
    
    public class Metadata_from_command_passed_to_produced_scheduled_fault : FutureEventsTest_InMemory
    {
        private IMessageMetadataEnvelop<IFault<RaiseScheduledDomainEventCommand>> _schedulingCommandFault;
        private ScheduleErrorInFutureCommand _command;
        private IMessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<JobFailed> _jobFailedEnvelop;

      [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "12",1);
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.Prepare(_command, _commandMetadata)
                                    .Expect<JobFailed>()
                                    .And<IFault<RaiseScheduledDomainEventCommand>>()
                                    .Execute(TimeSpan.FromSeconds(30),false);

            _schedulingCommandFault = res.Message<IMessageMetadataEnvelop<IFault<RaiseScheduledDomainEventCommand>>>();
            _jobFailedEnvelop = res.Message<IMessageMetadataEnvelop<JobFailed>>();
        }

        [Fact]
        public void Result_contains_metadata()
        {
            Assert.NotNull(_schedulingCommandFault.Metadata);
        }

        [Fact]
        public void Result_contains_message()
        {
            Assert.NotNull(_schedulingCommandFault.Message);
        }

        [Fact]
        public void Result_message_has_expected_type()
        {
            Assert.IsAssignableFrom<IFault<RaiseScheduledDomainEventCommand>>(_schedulingCommandFault.Message);
        }

        [Fact]
        public void Result_message_has_expected_id()
        {
           Assert.Equal((_jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id, _schedulingCommandFault.Message.Message.Id);
        }

        [Fact]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
           Assert.Equal((_jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id, _schedulingCommandFault.Metadata.CasuationId);
        }

        [Fact]
        public void Result_metadata_has_correlation_id_same_as_command_metadata()
        {
           Assert.Equal(_commandMetadata.CorrelationId, _schedulingCommandFault.Metadata.CorrelationId);
        }

        [Fact]
        public void Result_metadata_has_processed_history_filled_from_aggregate()
        {
           Assert.Equal(1, _schedulingCommandFault.Metadata.History?.Steps.Count);
        }

        [Fact]
        public void Result_metadata_has_processed_correct_filled_history_step()
        {
            var step = _schedulingCommandFault.Metadata.History.Steps.First();

           Assert.Equal(AggregateActorName.New<TestAggregate>(_command.AggregateId).Name, step.Who);
           Assert.Equal(AggregateActor<TestAggregate>.CommandRaisedAnError, step.Why);
           Assert.Equal(AggregateActor<TestAggregate>.CreatedFault, step.What);
        }
    }

  
}