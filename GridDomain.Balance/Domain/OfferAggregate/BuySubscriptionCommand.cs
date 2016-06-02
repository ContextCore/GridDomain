using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class BuySubscriptionCommand : Command
    {
        public DateTime ActivateDate;
        public Guid BusinessId;
        public Guid SubscriptionId;
    }
}