using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    class Given_created_and_message_received_and_transitioned_event_when_hydrating : HydrationSpecification<SagaDataAggregate<SoftwareProgrammingSagaData>>
    {
        private readonly Guid _sagaId;
        private readonly SoftwareProgrammingSaga _machine;
        private readonly SoftwareProgrammingSagaData _softwareProgrammingSagaData;
        private readonly GotTiredDomainEvent _message;

        public Given_created_and_message_received_and_transitioned_event_when_hydrating()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaData = new SoftwareProgrammingSagaData(_machine.Coding);
            _message = new GotTiredDomainEvent(Guid.NewGuid());
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaData>(_softwareProgrammingSagaData, _sagaId);

            yield return new SagaMessageReceivedEvent<SoftwareProgrammingSagaData>(_sagaId,
                _softwareProgrammingSagaData, 
                _machine.GotTired,
                _message);

            yield return new SagaTransitionEvent<SoftwareProgrammingSagaData>(_sagaId,_softwareProgrammingSagaData, _machine.Sleeping);
        }

        [Test]
        public void Then_State_is_taken_from_transition_event()
        {
            Assert.AreEqual(_softwareProgrammingSagaData, Aggregate.Data);
        }

        [Test]
        public void Then_received_messages_contains_message()
        {
            CollectionAssert.Contains(Aggregate.ReceivedMessages, _message);
        }
    }
}