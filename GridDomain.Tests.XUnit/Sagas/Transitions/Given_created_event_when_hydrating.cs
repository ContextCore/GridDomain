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
    public class Given_created_event_when_hydrating : AggregateTest<SagaStateAggregate<SoftwareProgrammingSagaState>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingSaga _machine;
        private SoftwareProgrammingSagaState _softwareProgrammingSagaState;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaState>(_softwareProgrammingSagaState, _sagaId);
        }

        [Fact]
        public void Given_created_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaState = new SoftwareProgrammingSagaState(_sagaId, _machine.Sleeping.Name);
            Init();
            //Then_State_is_taken_from_event()
            Assert.Equal(_softwareProgrammingSagaState, Aggregate.Data);
            //Then_Id_is_taken_from_event()
            Assert.Equal(_sagaId, Aggregate.Id);
        }
    }
}