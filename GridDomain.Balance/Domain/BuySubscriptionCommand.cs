using System;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain
{
    public class BuySubscriptionCommand:Command
    {
        public Guid BusinessId;
        public Guid SubscriptionId;
        public DateTime ActivateDate;
    }
}