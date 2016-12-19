using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Metadata
{
    [TestFixture]
    class Metadata_from_saga_received_event_passed_to_produced_commands : SoftwareProgrammingInstanceSagaTest
    {
        private Guid SagaId;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;
        private IMessageMetadataEnvelop<MakeCoffeCommand> _answer;
        private GotTiredEvent _gotTiredEvent;
        private MessageMetadata _gotTiredEventMetadata;

        [OneTimeSetUp]
        public async Task When_publishing_start_message()
        {
            SagaId = Guid.NewGuid();
            _gotTiredEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(),Guid.NewGuid(), SagaId);
            _gotTiredEventMetadata = new MessageMetadata(_gotTiredEvent.SourceId, BusinessDateTime.UtcNow, Guid.NewGuid(), Guid.NewGuid());

            var res = await GridNode.NewDebugWaiter(TimeSpan.FromHours(5))
                                    .Expect<IMessageMetadataEnvelop<MakeCoffeCommand>>()
                                    .Create()
                                    .Publish(_gotTiredEvent, _gotTiredEventMetadata);

            _answer = res.Message<IMessageMetadataEnvelop<MakeCoffeCommand>>();
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
            Assert.IsInstanceOf<MakeCoffeCommand>(_answer.Message);
        }

        [Test]
        public void Result_message_has_expected_value()
        {
            Assert.AreEqual(_gotTiredEvent.PersonId, _answer.Message.PersonId);
        }

        [Test]
        public void Result_metadata_has_command_id_as_casuation_id()
        {
            Assert.AreEqual(_gotTiredEvent.SourceId, _answer.Metadata.CasuationId);
        }

        [Test]
        public void Result_metadata_has_correlation_id_same_as_command_metadata()
        {
            Assert.AreEqual(_gotTiredEventMetadata.CorrelationId, _answer.Metadata.CorrelationId);
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
            var name = AggregateActorName.New<SagaDataAggregate<SoftwareProgrammingSagaData>>(SagaId);

            Assert.AreEqual(name.Name, step.Who);
            Assert.AreEqual(SagaActorLiterals.SagaProducedACommand, step.Why);
            Assert.AreEqual(SagaActorLiterals.PublishingCommand, step.What);
        }
    }
}