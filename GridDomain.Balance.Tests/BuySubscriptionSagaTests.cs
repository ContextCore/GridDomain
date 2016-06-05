using System;
using BusinessNews.Domain.Sagas.BuySubscription;
using NUnit.Framework;

namespace BusinessNews.Test
{
    [TestFixture]
    public class BuySubscriptionSagaTests
    {
        [Test]
        public void GetSagaGraph()
        {
            var sagaStateAggregate = new BuySubscriptionSagaStateAggregate
                                       (Guid.NewGuid(), BuySubscriptionSaga.State.SubscriptionSet);

            var saga = new BuySubscriptionSaga(sagaStateAggregate);
            var graph = saga.Machine.ToDotGraph();
            Console.WriteLine(graph);
        }

        [Test]
        public void Given_business_has_enough_money_when_purchase_subscription_it_is_ok()
        {
            var sagaStateAggregate = new BuySubscriptionSagaStateAggregate
                                       (Guid.NewGuid(), BuySubscriptionSaga.State.SubscriptionSet);

            var saga = new BuySubscriptionSaga(sagaStateAggregate);
            Guid businessId = Guid.NewGuid();

          //  var subscriptionOrderedEvent = new SubscriptionOrderedEvent();

           // saga.Handle(subscriptionOrderedEvent);
        }
    }
}