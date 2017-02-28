using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_created_event_when_hydrating : AggregateTest<SagaStateAggregate<SoftwareProgrammingSagaData>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingSaga _machine;
        private SoftwareProgrammingSagaData _softwareProgrammingSagaData;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaData>(_softwareProgrammingSagaData, _sagaId);
        }

        [Fact]
        public void Given_created_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaData = new SoftwareProgrammingSagaData(_sagaId, _machine.Sleeping.Name);
            Init();
            //Then_State_is_taken_from_event()
            Assert.Equal(_softwareProgrammingSagaData, Aggregate.Data);
            //Then_Id_is_taken_from_event()
            Assert.Equal(_sagaId, Aggregate.Id);
        }
    }
}