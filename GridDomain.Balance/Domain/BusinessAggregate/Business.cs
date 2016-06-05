using System;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.Balance.Domain.OfferAggregate;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    internal class Business : AggregateBase
    {
        private Guid BalanceId;

        public string Name;
        private Guid SubscriptionId;

        private readonly List<Guid> _subscriptionOrders = new List<Guid>();
        public IReadOnlyCollection<Guid> SubscriptionOrders => _subscriptionOrders; 

        public Business(Guid id, string name, Guid subscriptionId, Guid balanceId)
        {
            RaiseEvent(new BusinessCreatedEvent(id)
            {
                Names = name,
                SubscriptionId = subscriptionId,
                BalanceId = balanceId
            });
        }

        public void OrderSubscription(Guid suibscriptionId, Guid offerId)
        {
            RaiseEvent(new SubscriptionOrderedEvent(Id, suibscriptionId, offerId));
        }

        public void PurchaseSubscription(Guid subscriptionId)
        {
            RaiseEvent(new SubscriptionPurchasedEvent(Id,subscriptionId));
        }
        public void RevokeSubscription()
        {
            RaiseEvent(new SubscriptionRevokedEvent(Id, SubscriptionId));
        }


        private void Apply(SubscriptionRevokedEvent e)
        {
            SubscriptionId = FreeSubscription.ID;
        }

        private void Apply(SubscriptionPurchasedEvent e)
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
            BalanceId = e.BalanceId;
            SubscriptionId = e.SubscriptionId;
        }
    }

    public class SubscriptionOrderedEvent : DomainEvent
    {
        public Guid SuibscriptionId { get; }
        public Guid OfferId { get; }

        public SubscriptionOrderedEvent(Guid businessId, Guid suibscriptionId, Guid offerId):base(businessId)
        {
            SuibscriptionId = suibscriptionId;
            OfferId = offerId;
        }
    }
}