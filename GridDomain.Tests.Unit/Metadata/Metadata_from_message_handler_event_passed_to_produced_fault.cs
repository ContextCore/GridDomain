using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;

using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Metadata
{
    [TestFixture]
    class Metadata_from_message_handler_event_passed_to_produced_fault : SampleDomainCommandExecutionTests
    {
        private IMessageMetadataEnvelop<IFault<SampleAggregateCreatedEvent>> _answer;
        private CreateSampleAggregateCommand _command;
        private MessageMetadata _commandMetadata;
        private IMessageMetadataEnvelop<SampleAggregateCreatedEvent> _aggregateEvent;

        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(new SampleRouteMap(),
                                      r => r.RegisterHandler<SampleAggregateCreatedEvent, FaultyCreateProjectionBuilder>(nameof(DomainEvent.SourceId)));
        }

        [OneTimeSetUp]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            _command = new CreateSampleAggregateCommand(1, Guid.NewGuid());
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.Prepare(_command, _commandMetadata)
                                    .Expect<SampleAggregateCreatedEvent>()
                                    .And<IFault<SampleAggregateCreatedEvent>>()
                                    .Execute(null, false);

            _answer = res.MessageWithMetadata<IFault<SampleAggregateCreatedEvent>>();
            _aggregateEvent = res.MessageWithMetadata<SampleAggregateCreatedEvent>();
        }


        [Test]
        public void Result_contains_metadata()
        {
            Assert.NotNull(_answer.Metadata);
        }

        [Test]
        public void Result_message_has_expected_type()
        {
            Assert.IsInstanceOf<IFault<SampleAggregateCreatedEvent>>(_answer.Message);
        }

        [Test]
        public void Produced_fault_produced_by_projection_builder()
        {
            Assert.AreEqual(typeof(FaultyCreateProjectionBuilder), _answer.Message.Processor);
        }


        [Test]
        public void Result_message_has_expected_id()
        {
            Assert.AreEqual(_command.AggregateId, _answer.Message.Message.SourceId);
        }

        [Test]
        public void Result_metadata_has_aggregate_event_id_as_casuation_id()
        {
            Assert.AreEqual(_aggregateEvent.Metadata.MessageId, _answer.Metadata.CasuationId);
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

            Assert.AreEqual(nameof(FaultyCreateProjectionBuilder), step.Who);
            Assert.AreEqual(MessageHandlingStatuses.MessageProcessCasuedAnError, step.Why);
            Assert.AreEqual(MessageHandlingStatuses.PublishingFault, step.What);
        }
    }
}