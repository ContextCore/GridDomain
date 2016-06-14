using System;
using BusinessNews.Domain.BusinessAggregate;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaFactory : ISagaFactory<BuySubscriptionSaga, SubscriptionOrderedEvent>,
                                              ISagaFactory<BuySubscriptionSaga, BuySubscriptionSagaStateAggregate>,
                                              IEmptySagaFactory<BuySubscriptionSaga>


    {
        private readonly AggregateFactory _aggregateFactory;

        public BuySubscriptionSagaFactory(AggregateFactory aggregateFactory)
        {
            _aggregateFactory = aggregateFactory;
        }

        public BuySubscriptionSaga Create(SubscriptionOrderedEvent message)
        {
            return new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(message.SagaId));
        }

        public BuySubscriptionSaga Create(BuySubscriptionSagaStateAggregate message)
        {
            return new BuySubscriptionSaga(message);
        }

        public BuySubscriptionSaga Create()
        {
            return Create(_aggregateFactory.Build<BuySubscriptionSagaStateAggregate>(Guid.Empty));
        }
    }
}