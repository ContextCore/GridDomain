using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.XUnit.FutureEvents;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using Xunit;

namespace GridDomain.Tests.XUnit.Metadata
{
    
    public class Metadata_from_command_passed_to_produced_scheduled_event : FutureEventsTest_InMemory
    {
        private IMessageMetadataEnvelop<TestDomainEvent> _answer;
        private ScheduleEventInFutureCommand _command;
        private IMessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<JobSucceeded> _jobSucced;

      [Fact]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new ScheduleEventInFutureCommand(DateTime.Now.AddMilliseconds(20), Guid.NewGuid(), "12");
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.Prepare(_command, _commandMetadata)
                                    .Expect<TestDomainEvent>()
                                    .And<JobSucceeded>()
                                    .Execute(null, false);

            _answer = res.MessageWithMetadata<TestDomainEvent>();
            _jobSucced = res.MessageWithMetadata<JobSucceeded>();
        }

        [Fact]
        public void Result_contains_metadata()
        {
            Assert.NotNull(_answer.Metadata);
        }

        [Fact]
        public void Result_contains_message()
        {
            Assert.NotNull(_answer.Message);
        }

        [Fact]
        public void Result_message_has_expected_type()
        {
            Assert.IsAssignableFrom<TestDomainEvent>(_answer.Message);
        }

        [Fact]
        public void Result_message_has_expected_id()
        {
           Assert.Equal(_command.AggregateId, _answer.Message.SourceId);
        }

        [Fact]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
           Assert.Equal((_jobSucced.Message.Message as ICommand)?.Id, _answer.Metadata.CasuationId);
        }

        [Fact]
        public void Result_metadata_has_correlation_id_same_as_command_metadata()
        {
           Assert.Equal(_commandMetadata.CorrelationId, _answer.Metadata.CorrelationId);
        }

        [Fact]
        public void Result_metadata_has_processed_history_filled_from_aggregate()
        {
           Assert.Equal(1, _answer.Metadata.History?.Steps.Count);
        }

        [Fact]
        public void Result_metadata_has_processed_correct_filled_history_step()
        {
            var step = _answer.Metadata.History.Steps.First();

           Assert.Equal(AggregateActorName.New<TestAggregate>(_command.AggregateId).Name, step.Who);
           Assert.Equal(AggregateActor<TestAggregate>.CommandExecutionCreatedAnEvent, step.Why);
           Assert.Equal(AggregateActor<TestAggregate>.PublishingEvent, step.What);
        }
    }
}