using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using GridDomain.Domain.Tests;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Sagas.SagaStateAggregate
{
    [TestFixture]
    class Given_new_saga_state_When_transit :
        DomainEventsProductionSpecification<SagaStateAggregate<TestState, TestTransition>>
    {
        [Then]
        public void State_transition_events_should_be_raised()
        {
            VerifyRaisedEvents();
        }

        protected override void When()
        {
            Aggregate.StateChanged(TestTransition.Forward, TestState.Running);
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, Aggregate.Id);
        }

        protected override IEnumerable<DomainEvent> ExpectedEvents()
        {
            yield return
                new SagaTransitionEvent<TestState, TestTransition>(TestTransition.Forward, TestState.Running,
                    Aggregate.Id);
        }
    }
}
