using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain
{
    public class BuySubscriptionCommand : Command
    {
        public DateTime ActivateDate;
        public Guid BusinessId;
        public Guid SubscriptionId;
    }
}