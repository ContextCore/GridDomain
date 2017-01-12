using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.Transitions
{
    [TestFixture]
    class Given_created_and_message_received_when_hydrating : AggregateTest<SagaStateAggregate<SoftwareProgrammingSagaData>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingSaga _machine;
        private SoftwareProgrammingSagaData _softwareProgrammingSagaData;
        private GotTiredEvent _message;

        [OneTimeSetUp]
        public void Given_created_and_message_received_and_transitioned_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaData = new SoftwareProgrammingSagaData(_sagaId,_machine.Sleeping.Name);
            _message = new GotTiredEvent(Guid.NewGuid());
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaData>(_softwareProgrammingSagaData, _sagaId);

            yield return new SagaMessageReceivedEvent<SoftwareProgrammingSagaData>(_sagaId,
                                                                                   _softwareProgrammingSagaData, 
                                                                                   _machine.GotTired.Name,
                                                                                   _message);
        }

        [Test]
        public void Then_State_is_taken_from_message_received_event()
        {
            Assert.AreEqual(_softwareProgrammingSagaData, Aggregate.Data);
        }
    }
}