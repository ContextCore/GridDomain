using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Tests.Framework;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.SagaStateAggregate
{
    [TestFixture]
    internal class Given_created_event_should_hydrate_state :
        AggregateTest<SagaStateAggregate<TestState, TestTransition>>
    {
        private readonly Guid _sourceId = Guid.NewGuid();

        protected override IEnumerable<DomainEvent> Given()
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