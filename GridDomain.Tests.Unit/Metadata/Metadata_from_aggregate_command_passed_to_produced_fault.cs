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
    class Metadata_from_aggregate_command_passed_to_produced_fault : SampleDomainCommandExecutionTests
    {
        private IMessageMetadataEnvelop<IFault<AlwaysFaultAsyncCommand>> _answer;
        private AlwaysFaultAsyncCommand _command;
        private MessageMetadata _commandMetadata;

        [OneTimeSetUp]
        public async Task When_execute_aggregate_command_with_fault_and_metadata()
        {
            _command = new AlwaysFaultAsyncCommand(Guid.NewGuid(),TimeSpan.FromMilliseconds(50));
            _commandMetadata = new MessageMetadata(_command.Id, BusinessDateTime.Now, Guid.NewGuid());

            var res = await GridNode.Prepare(_command, _commandMetadata)
                                    .Expect<SampleAggregateCreatedEvent>()
                                    .Execute(null,false);

            _answer = res.Message<IMessageMetadataEnvelop<IFault<AlwaysFaultAsyncCommand>>>();
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
            Assert.IsInstanceOf<IFault<AlwaysFaultAsyncCommand>>(_answer.Message);
        }

        [Test]
        public void Result_message_has_expected_id()
        {
            Assert.AreEqual(_command.Id, _answer.Message.Message.Id);
        }

        [Test]
        public void Result_message_has_expected_value()
        {
            Assert.AreEqual(_command.GetType(), _answer.Message.Message.GetType());
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

            Assert.AreEqual(AggregateActorName.New<SampleAggregate>(_command.AggregateId).Name, step.Who);
            Assert.AreEqual(AggregateActor<SampleAggregate>.CommandRaisedAnError, step.Why);
            Assert.AreEqual(AggregateActor<SampleAggregate>.CreatedFault, step.What);
        }
    }
}