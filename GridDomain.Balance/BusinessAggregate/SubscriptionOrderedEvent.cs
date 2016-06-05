using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class SubscriptionOrderedEvent : DomainEvent
    {
        public Guid SuibscriptionId { get; }
        public Guid OfferId { get; }

        public Guid BusinessId => SourceId;
        public Guid AccountId { get; }

        public SubscriptionOrderedEvent(Guid businessId, Guid suibscriptionId, Guid offerId, Guid accountId):base(businessId)
        {
            SuibscriptionId = suibscriptionId;
            OfferId = offerId;
            AccountId = accountId;
        }
    }
}