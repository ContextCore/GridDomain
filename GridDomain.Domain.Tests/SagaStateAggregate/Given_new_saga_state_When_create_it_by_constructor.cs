using System;
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
    class Given_new_saga_state_When_create_it_by_constructor :
        DomainEventsProductionSpecification<SagaStateAggregate<TestState, TestTransition>>
    {
        [Then]
        public void SagaCreatedEvent_should_be_raised()
        {
            VerifyRaisedEvents();
        }


        protected override SagaStateAggregate<TestState, TestTransition> CreateAggregate()
        {
            return new SagaStateAggregate<TestState, TestTransition>(Guid.NewGuid(), TestState.Idle);
        }

        protected override IEnumerable<DomainEvent> ExpectedEvents()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, Aggregate.Id);

        }
    }
}
