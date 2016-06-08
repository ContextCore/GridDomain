using System;
using System.Collections.Generic;
using BusinessNews.Domain.OfferAggregate;
using CommonDomain.Core;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class Business : AggregateBase
    {
        //TODO: decide should we extend orders with order\offer\subscription Id or store it in saga
        private readonly List<Guid> _subscriptionOrders = new List<Guid>();
        private Guid MainAccountId;
        private Guid MediaAccountId;

        public string Name { get; private set; }
        private Guid SubscriptionId;

        public Business(Guid id, string name, Guid subscriptionId, Guid balanceId)
        {
            RaiseEvent(new BusinessCreatedEvent(id)
            {
                Names = name,
                SubscriptionId = subscriptionId,
                BalanceId = balanceId
            });
        }

        private Business(Guid id)
        {
            Id = id;
        }

        public IReadOnlyCollection<Guid> SubscriptionOrders => _subscriptionOrders;

        public void OrderSubscription(Guid suibscriptionId, Guid offerId)
        {
            RaiseEvent(new SubscriptionOrderedEvent(Id, suibscriptionId, offerId, MainAccountId));
        }

        public void PurchaseSubscription(Guid subscriptionId)
        {
            RaiseEvent(new SubscriptionOrderCompletedEvent(Id, subscriptionId));
        }

        public void RevokeSubscription()
        {
            RaiseEvent(new SubscriptionRevokedEvent(Id, SubscriptionId));
        }


        private void Apply(SubscriptionRevokedEvent e)
        {
            SubscriptionId = FreeSubscription.ID;
        }

        private void Apply(SubscriptionOrderCompletedEvent e)
        {
            SubscriptionId = e.SubscriptionId;
        }

        private void Apply(SubscriptionOrderedEvent e)
        {
            _subscriptionOrders.Add(e.SuibscriptionId);
        }

        private void Apply(BusinessCreatedEvent e)
        {
            Id = e.SourceId;
            MediaAccountId = e.BalanceId;
            SubscriptionId = e.SubscriptionId;
        }
    }
}