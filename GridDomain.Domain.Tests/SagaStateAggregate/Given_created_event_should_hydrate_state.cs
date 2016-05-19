using System;
using System.Collections.Generic;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using GridDomain.Domain.Tests;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Sagas.SagaStateAggregate
{
    [TestFixture]
    class Given_created_event_should_hydrate_state : HydrationSpecification<SagaStateAggregate<TestState, TestTransition>>
    {
        private readonly Guid _sourceId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, _sourceId);
        }

        [Then]
        public void State_should_be_taken_from_event()
        {
            Assert.AreEqual(Aggregate.MachineState, TestState.Idle);
        }


        [Then]
        public void Id_should_be_taken_from_event()
        {
            Assert.AreEqual(Aggregate.Id, _sourceId);
        }

    }
}