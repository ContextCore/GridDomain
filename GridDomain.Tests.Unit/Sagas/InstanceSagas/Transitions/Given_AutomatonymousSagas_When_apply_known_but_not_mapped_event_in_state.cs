using System;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.Transitions
{
    [TestFixture]
    internal class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state(Given_AutomatonymousSaga given)
        {
            _given = given;
        }

        public Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state()
            :this(new Given_AutomatonymousSaga(m => m.Sleeping))
        {
        }

        private readonly Given_AutomatonymousSaga _given;
        private static GotTiredEvent _gotTiredEvent;

        private static async Task When_apply_known_but_not_mapped_event_in_state(ISagaInstance sagaInstance)
        {
            _gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            await sagaInstance.Transit(_gotTiredEvent);
        }

        
        [Then]
        public async Task Transition_raises_an_error()
        {
            Assert.ThrowsAsync<SagaTransitionException>(async () => await When_apply_known_but_not_mapped_event_in_state(_given.SagaInstance));
        }
    }
}