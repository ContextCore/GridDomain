using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.Transitions
{
    class Given_created_event_when_hydrating: HydrationSpecification<SagaDataAggregate<SoftwareProgrammingSagaData>>
    {
        private readonly Guid _sagaId;
        private readonly SoftwareProgrammingSaga _machine;
        private readonly SoftwareProgrammingSagaData _softwareProgrammingSagaData;

        public Given_created_event_when_hydrating()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaData = new SoftwareProgrammingSagaData(_machine.Sleeping.Name);
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaData>(
                _softwareProgrammingSagaData, _sagaId);
        }

        [Test]
        public void Then_State_is_taken_from_event()
        {
            Assert.AreEqual(_softwareProgrammingSagaData,Aggregate.Data);
        }

        [Test]
        public void Then_Id_is_taken_from_event()
        {
            Assert.AreEqual(_sagaId, Aggregate.Id);
        }
    }
}