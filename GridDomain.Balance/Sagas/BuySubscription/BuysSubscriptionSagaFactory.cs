using System;
using BusinessNews.Domain.BusinessAggregate;
using GridDomain.Node.Actors;

namespace BusinessNews.Domain.Sagas.BuySubscription
{
    public class BuySubscriptionSagaFactory : ISagaFactory<BuySubscriptionSaga, SubscriptionOrderedEvent>,
                                              ISagaFactory<BuySubscriptionSaga, BuySubscriptionSagaStateAggregate>


    {
        public BuySubscriptionSaga Create(SubscriptionOrderedEvent message)
        {
            return new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(Guid.NewGuid()));
        }

        public BuySubscriptionSaga Create(BuySubscriptionSagaStateAggregate message)
        {
            return new BuySubscriptionSaga(message);
        }
    }
}