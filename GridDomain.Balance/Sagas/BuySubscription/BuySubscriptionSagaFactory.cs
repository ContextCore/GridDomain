using System;
using BusinessNews.Domain.BusinessAggregate;
using GridDomain.EventSourcing.Sagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaFactory : ISagaFactory<BuySubscriptionSaga, SubscriptionOrderedEvent>,
                                              ISagaFactory<BuySubscriptionSaga, BuySubscriptionSagaStateAggregate>,
                                              IEmptySagaFactory<BuySubscriptionSaga>


    {
        public BuySubscriptionSaga Create(SubscriptionOrderedEvent message)
        {
            return new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(Guid.NewGuid()));
        }

        public BuySubscriptionSaga Create(BuySubscriptionSagaStateAggregate message)
        {
            return new BuySubscriptionSaga(message);
        }

        public BuySubscriptionSaga Create()
        {
            return Create(new BuySubscriptionSagaStateAggregate(Guid.Empty));
        }
    }
}