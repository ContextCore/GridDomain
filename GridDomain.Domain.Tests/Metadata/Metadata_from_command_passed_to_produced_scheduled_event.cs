using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using GridDomain.Tests.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Metadata
{
    [TestFixture]
    class Metadata_from_command_passed_to_produced_scheduled_event : FutureEventsTest_InMemory
    {
        private IMessageMetadataEnvelop<TestDomainEvent> _answer;
        private ScheduleEventInFutureCommand _command;
        private IMessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<JobSucceeded> _jobSucced;

        [OneTimeSetUp]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new ScheduleEventInFutureCommand(DateTime.Now.AddMilliseconds(20), Guid.NewGuid(), "12");
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.NewCommandWaiter(TimeSpan.FromHours(1), false)
                                    .Expect<IMessageMetadataEnvelop<TestDomainEvent>>()
                                    .And<IMessageMetadataEnvelop<JobSucceeded>>()
                                    .Create()
                                    .Execute(_command, _commandMetadata);

            _answer = res.Message<IMessageMetadataEnvelop<TestDomainEvent>>();
            _jobSucced = res.Message<IMessageMetadataEnvelop<JobSucceeded>>();
        }

        [Test]
        public void Result_contains_metadata()
        {
            Assert.NotNull(_answer.Metadata);
        }

        [Test]
        public void Result_contains_message()
        {
            Assert.NotNull(_answer.Message);
        }

        [Test]
        public void Result_message_has_expected_type()
        {
            Assert.IsInstanceOf<TestDomainEvent>(_answer.Message);
        }

        [Test]
        public void Result_message_has_expected_id()
        {
            Assert.AreEqual(_command.AggregateId, _answer.Message.SourceId);
        }

        [Test]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
            Assert.AreEqual((_jobSucced.Message.Message as ICommand)?.Id, _answer.Metadata.CasuationId);
        }

        [Test]
        public void Result_metadata_has_correlation_id_same_as_command_metadata()
        {
            Assert.AreEqual(_commandMetadata.CorrelationId, _answer.Metadata.CorrelationId);
        }

        [Test]
        public void Result_metadata_has_processed_history_filled_from_aggregate()
        {
            Assert.AreEqual(1, _answer.Metadata.History?.Steps.Count);
        }

        [Test]
        public void Result_metadata_has_processed_correct_filled_history_step()
        {
            var step = _answer.Metadata.History.Steps.First();

            Assert.AreEqual(AggregateActorName.New<TestAggregate>(_command.AggregateId).Name, step.Who);
            Assert.AreEqual(AggregateActor<TestAggregate>.CommandExecutionCreatedAnEvent, step.Why);
            Assert.AreEqual(AggregateActor<TestAggregate>.PublishingEvent, step.What);
        }
    }
}