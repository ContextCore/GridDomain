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
    internal class Given_transitioned_event_should_hydrate_state :
        AggregateTest<SagaStateAggregate<TestState, TestTransition>>
    {
        private readonly Guid _sourceId = Guid.NewGuid();

        [OneTimeSetUp]
        public void Given_transitioned_event()
        {
            Init();    
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, _sourceId);
            yield return new SagaTransitionEvent<TestState, TestTransition>(TestTransition.Forward, TestState.Running, _sourceId);
        }

        [Then]
        public void State_should_be_taken_from_event()
        {
            Assert.AreEqual(Aggregate.MachineState, TestState.Running);
        }
    }
}