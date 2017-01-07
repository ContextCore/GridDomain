using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.Transitions
{
    [TestFixture]
    public class Given_created_event_when_hydrating: AggregateTest<SagaDataAggregate<SoftwareProgrammingSagaData>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingSaga _machine;
        private SoftwareProgrammingSagaData _softwareProgrammingSagaData;

        [OneTimeSetUp]
        public void Given_created_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaData = new SoftwareProgrammingSagaData(_machine.Sleeping.Name);
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
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