using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_created_and_message_received_when_hydrating :
        AggregateTest<SagaStateAggregate<SoftwareProgrammingSagaState>>
    {
        private Guid _sagaId;
        private SoftwareProgrammingSaga _machine;
        private SoftwareProgrammingSagaState _softwareProgrammingSagaState;
        private GotTiredEvent _message;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<SoftwareProgrammingSagaState>(_softwareProgrammingSagaState, _sagaId);

            yield return
                new SagaMessageReceivedEvent<SoftwareProgrammingSagaState>(_sagaId,
                                                                          _softwareProgrammingSagaState,
                                                                          _machine.GotTired.Name,
                                                                          _message);
        }

        [Fact]
        public void Given_created_and_message_received_and_transitioned_event()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SoftwareProgrammingSaga();
            _softwareProgrammingSagaState = new SoftwareProgrammingSagaState(_sagaId, _machine.Sleeping.Name);
            _message = new GotTiredEvent(Guid.NewGuid());
            Init();
            //Then_State_is_taken_from_message_received_event()
            Assert.Equal(_softwareProgrammingSagaState, Aggregate.SagaState);
        }
    }
}