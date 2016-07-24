using System;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.InstanceSagas.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas.Transitions
{
    [TestFixture]
    internal class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state(Given_State_SoftareProgramming_Saga given)
        {
            _given = given;
        }

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state()
            :this(new Given_State_SoftareProgramming_Saga(m => m.Sleeping))
        {
        }

        private readonly Given_State_SoftareProgramming_Saga _given;
        private static GotTiredDomainEvent _gotTiredDomainEvent;
        private IAggregate SagaDataAggregate => _given.SagaDataAggregate;

        private static void When_apply_known_but_not_mapped_event_in_state(ISagaInstance sagaInstance)
        {
            _gotTiredDomainEvent = new GotTiredDomainEvent(Guid.NewGuid());
            sagaInstance.Transit(_gotTiredDomainEvent);
        }

        [Then]
        public void State_not_changed()
        {
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            Assert.AreEqual(_given.SagaMachine.Sleeping.Name, _given.SagaDataAggregate.Data.CurrentStateName);
        }

        [Then]
        public void State_events_containes_received_message()
        {
            SagaDataAggregate.ClearUncommittedEvents();
            When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance);
            var @event = SagaDataAggregate.GetUncommittedEvents().OfType<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>().First();
            Assert.AreEqual(_gotTiredDomainEvent, @event.Message);
        }
    }
}