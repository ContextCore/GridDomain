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
        public Guid AccountId { get; private set; }

        public string Name { get; private set; }
        public Guid SubscriptionId { get; private set; }

        public Business(Guid id, string name, Guid subscriptionId, Guid balanceId)
        {
            RaiseEvent(new BusinessCreatedEvent(id)
            {
                Name = name,
                SubscriptionId = subscriptionId,
                AccountId = balanceId
            });
        }

        private Business(Guid id)
        {
            Id = id;
        }

        public IReadOnlyCollection<Guid> SubscriptionOrders => _subscriptionOrders;

        public void OrderSubscription(Guid suibscriptionId, Guid offerId)
        {
            RaiseEvent(new SubscriptionOrderedEvent(Id, suibscriptionId, offerId, AccountId));
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
            Name = e.Name;
            AccountId = e.AccountId;
            SubscriptionId = e.SubscriptionId;
        }
    }
}