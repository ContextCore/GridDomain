using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSagas_When_apply_known_but_not_mapped_event_in_state
    {
        [Fact]
        public async Task When_apply_known_but_not_mapped_event_in_state()
        {
            var given = new Given_AutomatonymousSaga(m => m.Sleeping);
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            //Transition_raises_an_error()
            await given.SagaInstance.Transit(gotTiredEvent)
                       .ShouldThrow<SagaTransitionException>();
        }
    }
}