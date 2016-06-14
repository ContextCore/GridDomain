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
        private T ExpectCommand<T>(IDomainSaga saga) where T : class, ICommand
        {
            var cmd = saga.CommandsToDispatch.FirstOrDefault();
            Assert.NotNull(cmd, $"Cannot find expected {typeof (T).Name} in dispatched messages");
            var typedCommand = cmd as T;
            Assert.NotNull(cmd, $"Dispatched message has wrong type: {cmd.GetType().Name}. Expected: {typeof (T).Name}");
            saga.ClearCommandsToDispatch();
            return typedCommand;
        }

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
        public void Given_business_with_enough_balance_then_saga_is_working_on_aggregates()
        {
        }

        [Test]
        public void Given_positive_flow_events_saga_completes()
        {
            var sagaStateAggregate = new BuySubscriptionSagaStateAggregate
                (Guid.NewGuid(), BuySubscriptionSaga.State.SubscriptionSet);

            var saga = new BuySubscriptionSaga(sagaStateAggregate);

            var data = new Fixture();

            var subscriptionOrderedEvent = data.Build<SubscriptionOrderedEvent>()
                .With(e => e.OfferId, WellKnownOffers.Catalog.Keys.RandomElement())
                .Create();

            var subscriptionId = subscriptionOrderedEvent.SuibscriptionId;
            var offerId = subscriptionOrderedEvent.OfferId;
            var accountId = subscriptionOrderedEvent.AccountId;
            var businessId = subscriptionOrderedEvent.BusinessId;
            saga.Transit(subscriptionOrderedEvent);

            var createSubscription = ExpectCommand<CreateSubscriptionCommand>(saga);
            Assert.AreEqual(subscriptionId, createSubscription.SubscriptionId);
            Assert.AreEqual(offerId, createSubscription.Offer);
            Assert.AreEqual(BuySubscriptionSaga.State.SubscriptionCreating, saga.DomainState);


            var offer = WellKnownOffers.Catalog[offerId];
            var subscriptionCreatedEvent = new SubscriptionCreatedEvent(subscriptionId, offer);
            saga.Transit(subscriptionCreatedEvent);

            var createBillCommand = ExpectCommand<ChargeSubscriptionCommand>(saga);
            Assert.AreEqual(subscriptionId, createBillCommand.SubscriptionId);
            var chargeId = createBillCommand.ChargeId;

            var subscriptionChargedEvent = new SubscriptionChargedEvent(subscriptionId, chargeId, offer.Price);
            saga.Transit(subscriptionChargedEvent);

            var charge = new Charge(subscriptionChargedEvent.ChargeId, subscriptionChargedEvent.Price);
            var billCreateCommand = ExpectCommand<CreateBillCommand>(saga);
            Assert.AreEqual(new[] {charge}, billCreateCommand.Charges);
            var billId = billCreateCommand.BillId;

            var billCreatedEvent = new BillCreatedEvent(billId, new[] {charge}, charge.Amount);
            saga.Transit(billCreatedEvent);

            var payBillCommand = ExpectCommand<PayForBillCommand>(saga);
            Assert.AreEqual(billId, payBillCommand.BillId);
            Assert.AreEqual(offer.Price, payBillCommand.Amount);
            Assert.AreEqual(accountId, payBillCommand.AccountId);

            var billPaidEvent = new BillPayedEvent(billId);
            saga.Transit(billPaidEvent);

            var subscriptionChangeCommand = ExpectCommand<CompleteBusinessSubscriptionOrderCommand>(saga);
            Assert.AreEqual(subscriptionId, subscriptionChangeCommand.SubscriptionId);
            Assert.AreEqual(businessId, subscriptionChangeCommand.BusinessId);

            var orderCompletedEvent = new SubscriptionOrderCompletedEvent(businessId, subscriptionId);
            saga.Transit(orderCompletedEvent);

            Assert.AreEqual(BuySubscriptionSaga.State.SubscriptionSet, saga.DomainState);
        }
    }
}