using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_created_event_when_hydrating : AggregateTest<SagaStateAggregate<SoftwareProgrammingState>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingProcess _machine;
        private SoftwareProgrammingState _softwareProgrammingState;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreated<SoftwareProgrammingState>(_softwareProgrammingState, _sagaId);
        }

        [Fact]
        public void Given_created_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingProcess();
            _softwareProgrammingState = new SoftwareProgrammingState(_sagaId, _machine.Sleeping.Name);
            Init();
            //Then_State_is_taken_from_event()
            Assert.Equal(_softwareProgrammingState, Aggregate.State);
            //Then_Id_is_taken_from_event()
            Assert.Equal(_sagaId, Aggregate.Id);
        }
    }
}