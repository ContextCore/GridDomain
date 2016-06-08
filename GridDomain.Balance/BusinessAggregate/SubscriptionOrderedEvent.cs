using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class SubscriptionOrderedEvent : DomainEvent
    {
        public SubscriptionOrderedEvent(Guid businessId, Guid suibscriptionId, Guid offerId, Guid accountId)
            : base(businessId)
        {
            SuibscriptionId = suibscriptionId;
            OfferId = offerId;
            AccountId = accountId;
        }

        public Guid SuibscriptionId { get; }
        //set for tests
        public Guid OfferId { get; set; }

        public Guid BusinessId => SourceId;
        public Guid AccountId { get; }
    }
}