using System;
using BusinessNews.Domain.BusinessAggregate;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaFactory : ISagaFactory<BuySubscriptionSaga, SubscriptionOrderedEvent>,
                                              ISagaFactory<BuySubscriptionSaga, BuySubscriptionSagaStateAggregate>,
                                              ISagaFactory<BuySubscriptionSaga, Guid>


    {
        private readonly AggregateFactory _aggregateFactory;

        public BuySubscriptionSagaFactory(AggregateFactory aggregateFactory)
        {
            _aggregateFactory = aggregateFactory;
        }

        public BuySubscriptionSaga Create(SubscriptionOrderedEvent state)
        {
            return new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(state.SagaId));
        }

        public BuySubscriptionSaga Create(BuySubscriptionSagaStateAggregate state)
        {
            return new BuySubscriptionSaga(state);
        }

        public BuySubscriptionSaga Create(Guid id)
        {
            return Create(_aggregateFactory.Build<BuySubscriptionSagaStateAggregate>(id));
        }
    }
}