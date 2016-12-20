using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Metadata
{
    [TestFixture]
    class Metadata_from_command_passed_to_produced_scheduled_fault : FutureEventsTest_InMemory
    {
        private IMessageMetadataEnvelop<IFault<RaiseScheduledDomainEventCommand>> _schedulingCommandFault;
        private ScheduleErrorInFutureCommand _command;
        private IMessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<JobFailed> _jobFailedEnvelop;

        [OneTimeSetUp]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new ScheduleErrorInFutureCommand(DateTime.Now.AddSeconds(0.5), Guid.NewGuid(), "12");
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.PrepareCommand(_command, _commandMetadata)
                                    .Expect<JobFailed>()
                                    .And<IFault<RaiseScheduledDomainEventCommand>>()
                                    .Execute(TimeSpan.FromSeconds(30));

            _schedulingCommandFault = res.Message<IMessageMetadataEnvelop<IFault<RaiseScheduledDomainEventCommand>>>();
            _jobFailedEnvelop = res.Message<IMessageMetadataEnvelop<JobFailed>>();
        }

        [Test]
        public void Result_contains_metadata()
        {
            Assert.NotNull(_schedulingCommandFault.Metadata);
        }

        [Test]
        public void Result_contains_message()
        {
            Assert.NotNull(_schedulingCommandFault.Message);
        }

        [Test]
        public void Result_message_has_expected_type()
        {
            Assert.IsInstanceOf<IFault<RaiseScheduledDomainEventCommand>>(_schedulingCommandFault.Message);
        }

        [Test]
        public void Result_message_has_expected_id()
        {
            Assert.AreEqual((_jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id, _schedulingCommandFault.Message.Message.Id);
        }

        [Test]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
            Assert.AreEqual((_jobFailedEnvelop.Message.ProcessingMessage as ICommand)?.Id, _schedulingCommandFault.Metadata.CasuationId);
        }

        [Test]
        public void Result_metadata_has_correlation_id_same_as_command_metadata()
        {
            Assert.AreEqual(_commandMetadata.CorrelationId, _schedulingCommandFault.Metadata.CorrelationId);
        }

        [Test]
        public void Result_metadata_has_processed_history_filled_from_aggregate()
        {
            Assert.AreEqual(1, _schedulingCommandFault.Metadata.History?.Steps.Count);
        }

        [Test]
        public void Result_metadata_has_processed_correct_filled_history_step()
        {
            var step = _schedulingCommandFault.Metadata.History.Steps.First();

            Assert.AreEqual(AggregateActorName.New<TestAggregate>(_command.AggregateId).Name, step.Who);
            Assert.AreEqual(AggregateActor<TestAggregate>.CommandRaisedAnError, step.Why);
            Assert.AreEqual(AggregateActor<TestAggregate>.CreatedFault, step.What);
        }
    }

  
}