using System;
using System.Linq;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.OfferAggregate;
using BusinessNews.Domain.Sagas.BuySubscription;
using BusinessNews.Domain.SubscriptionAggregate;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Acceptance;
using NUnit.Framework;
using Ploeh.AutoFixture;

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
        
            var data = new Fixture();

            var subscriptionOrderedEvent = data.Build<SubscriptionOrderedEvent>()
                                               .With(e => e.OfferId,WellKnownOffers.Catalog.Keys.RandomElement())
                                               .Create();

            var subscriptionId = subscriptionOrderedEvent.SuibscriptionId;
            var offerId = subscriptionOrderedEvent.OfferId;
            var accountId = subscriptionOrderedEvent.AccountId;
            var businessId = subscriptionOrderedEvent.BusinessId;
            saga.Handle(subscriptionOrderedEvent);

            var createSubscription = ExpectCommand<CreateSubscriptionCommand>(saga);
            Assert.AreEqual(subscriptionId, createSubscription.SubscriptionId);
            Assert.AreEqual(offerId, createSubscription.Offer);
            Assert.AreEqual(BuySubscriptionSaga.State.SubscriptionCreating, saga.DomainState);


            var offer = WellKnownOffers.Catalog[offerId];
            var subscriptionCreatedEvent = new SubscriptionCreatedEvent(subscriptionId, offer);
            saga.Handle(subscriptionCreatedEvent);

            var createBillCommand = ExpectCommand<CreateSubscriptionBillCommand>(saga);
            Assert.AreEqual(subscriptionId,createBillCommand.SubscriptionId);
            var billId = createBillCommand.BillId;

            var billCreateEvent = new SubscriptionBillCreatedEvent(subscriptionId,billId,offer.Price);
            saga.Handle(billCreateEvent);

            var payBillCommand = ExpectCommand<PayForBillCommand>(saga);
            Assert.AreEqual(billId,payBillCommand.BillId);
            Assert.AreEqual(offer.Price,payBillCommand.Amount);
            Assert.AreEqual(accountId,payBillCommand.AccountId);

            var billPaidEvent = new BillPayedEvent(billId);
            saga.Handle(billPaidEvent);

            var subscriptionChangeCommand = ExpectCommand<CompleteBusinessSubscriptionOrderCommand>(saga);
            Assert.AreEqual(subscriptionId, subscriptionChangeCommand.SubscriptionId);
            Assert.AreEqual(businessId, subscriptionChangeCommand.BusinessId);

            var orderCompletedEvent = new SubscriptionOrderCompletedEvent(businessId,subscriptionId);
            saga.Handle(orderCompletedEvent);

            Assert.AreEqual(BuySubscriptionSaga.State.SubscriptionSet, saga.DomainState);
        }

        private T ExpectCommand<T>(Saga saga) where T: class, ICommand
        {
            var cmd = saga.MessagesToDispatch.FirstOrDefault();
            Assert.NotNull(cmd, $"Cannot find expected {typeof(T).Name} in dispatched messages");
            var typedCommand = cmd as T;
            Assert.NotNull(cmd, $"Dispatched message has wrong type: {cmd.GetType().Name}. Expected: {typeof(T).Name}");
            saga.MessagesToDispatch.Clear();
            return typedCommand;
        }
    }
}