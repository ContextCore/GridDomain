using System;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.Transitions
{
    [TestFixture]
    internal class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state(Given_State_SoftareProgramming_Saga given)
        {
            _given = given;
        }

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state()
            :this(new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.Sleeping))
        {
        }

        private readonly Given_State_SoftareProgramming_Saga _given;
        private static GotTiredEvent _gotTiredDomainEvent;
        private IAggregate SagaDataAggregate => _given.SagaDataAggregate;

        private static void When_apply_known_but_not_mapped_event_in_state(ISagaInstance sagaInstance)
        {
            _gotTiredDomainEvent = new GotTiredEvent(Guid.NewGuid());
            sagaInstance.Transit(_gotTiredDomainEvent);
        }

        [Then]
        public void State_not_changed()
        {
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            Assert.AreEqual(SoftwareProgrammingSaga.States.Sleeping, _given.SagaDataAggregate.MachineState);
        }
    }
}