using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class BuySubscriptionCommand : Command
    {
        public DateTime ActivateDate;
        public Guid BusinessId;
        public Guid SubscriptionId;
    }


}