using System;
using System.Collections.Generic;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using GridDomain.Domain.Tests;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Sagas.SagaStateAggregate
{
    [TestFixture]
    class Given_transitioned_event_should_hydrate_state : HydrationSpecification<SagaStateAggregate<TestState, TestTransition>>
    {
        private readonly Guid _sourceId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, _sourceId);
            yield return new SagaTransitionEvent<TestState,TestTransition>(TestTransition.Forward, TestState.Running, _sourceId);
        }

        [Then]
        public void State_should_be_taken_from_event()
        {
            Assert.AreEqual(Aggregate.MachineState, TestState.Running);
        }
    }
}