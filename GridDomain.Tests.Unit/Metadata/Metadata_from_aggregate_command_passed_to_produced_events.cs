using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;

using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Metadata
{
    [TestFixture]
    class Metadata_from_aggregate_command_passed_to_produced_events : SampleDomainCommandExecutionTests
    {
        private IMessageMetadataEnvelop<SampleAggregateCreatedEvent> _answer;
        private CreateSampleAggregateCommand _command;
        private MessageMetadata _commandMetadata;

        [OneTimeSetUp]
        public async Task When_execute_aggregate_command_with_metadata()
        {
            _command = new CreateSampleAggregateCommand(1,Guid.NewGuid());
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.Prepare(_command, _commandMetadata)
                                    .Expect<SampleAggregateCreatedEvent>()
                                    .Execute();

            _answer = res.MessageWithMetadata<SampleAggregateCreatedEvent>();
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
            Assert.IsInstanceOf<SampleAggregateCreatedEvent>(_answer.Message);
        }

        [Test]
        public void Result_message_has_expected_id()
        {
            Assert.AreEqual(_command.AggregateId, _answer.Message.SourceId);
        }

        [Test]
        public void Result_message_has_expected_value()
        {
            Assert.AreEqual(_command.Parameter.ToString(), _answer.Message.Value);
        }

        [Test]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
            Assert.AreEqual(_command.Id, _answer.Metadata.CasuationId);
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

            Assert.AreEqual(AggregateActorName.New<SampleAggregate>(_command.AggregateId).Name,step.Who);
            Assert.AreEqual(AggregateActor<SampleAggregate>.CommandExecutionCreatedAnEvent,step.Why);
            Assert.AreEqual(AggregateActor<SampleAggregate>.PublishingEvent,step.What);
        }

    }
}
