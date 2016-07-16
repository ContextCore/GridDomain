using System;
using System.Linq;
using System.Threading;
using CommonDomain;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    class Given_saga_When_publishing_start_message : InMemorySampleDomainTests   {
        private Guid _sagaId;
        private DomainEvent[] _sagaDataEvents;
        private SagaCreatedEvent<SoftwareProgrammingSagaData> _startedEvent;
        private SagaMessageReceivedEvent<SoftwareProgrammingSagaData> _sagaDomainMessageRecievedEvent;
        private SagaTransitionEvent<SoftwareProgrammingSagaData> _transitionEvent;
        private GotTiredDomainEvent _sagaStartMessage;

        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutes();
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();
            return new CustomContainerConfiguration(
                c => c.RegisterSaga<SoftwareProgrammingSaga,
                                    SoftwareProgrammingSagaData,
                                    GotTiredDomainEvent,
                                    SoftwareProgrammingSagaFactory
                                    >(),
                c => c.Register(baseConf)
                );
        }

        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            var generator = new Fixture();
            _sagaStartMessage = (GotTiredDomainEvent)
                new GotTiredDomainEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid())//generator.Create<GotTiredDomainEvent>()
                                         .CloneWithSaga(Guid.NewGuid());

            _sagaId = _sagaStartMessage.SagaId;
            GridNode.Transport.Publish(_sagaStartMessage);

            Thread.Sleep(100000);

            var sagaData = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(_sagaId);
            _sagaDataEvents = ((IAggregate)sagaData).GetUncommittedEvents().Cast<DomainEvent>().ToArray();

            _startedEvent = _sagaDataEvents.OfType<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                       .FirstOrDefault();

            _sagaDomainMessageRecievedEvent = _sagaDataEvents.OfType<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>()
                                                             .FirstOrDefault();

            _transitionEvent = _sagaDataEvents.OfType<SagaTransitionEvent<SoftwareProgrammingSagaData>>()
                .FirstOrDefault();
        }

        [Then]
        public void Saga_is_started()
        {
            Assert.NotNull(_startedEvent);
        }

        [Then]
        public void Saga_has_correct_id()
        {
            Assert.AreEqual(_startedEvent.SagaId, _sagaStartMessage.SagaId);
        }
    }
}
