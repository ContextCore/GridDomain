using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    class Given_created_event_when_hydrating: HydrationSpecification<SagaDataAggregate<SubscriptionRenewSagaData>>
    {
        private readonly Guid _sagaId;
        private readonly SubscriptionRenewSaga _machine;
        private readonly SubscriptionRenewSagaData _subscriptionRenewSagaData;

        public Given_created_event_when_hydrating()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SubscriptionRenewSaga();
            _subscriptionRenewSagaData = new SubscriptionRenewSagaData(_machine.Sleeping);
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<SubscriptionRenewSagaData>(_subscriptionRenewSagaData, _sagaId);
        }

        [Test]
        public void Then_State_is_taken_from_event()
        {
            Assert.AreEqual(_subscriptionRenewSagaData,Aggregate.Data);
        }

        [Test]
        public void Then_Id_is_taken_from_event()
        {
            Assert.AreEqual(_sagaId, Aggregate.Id);
        }
    }
}