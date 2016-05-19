using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using NUnit.Framework;

namespace GridDomain.Tests.SagaStateAggregate
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
