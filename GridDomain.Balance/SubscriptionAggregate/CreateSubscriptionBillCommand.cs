using System;
using GridDomain.CQRS;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class CreateSubscriptionBillCommand : Command
    {
        public Guid SubscriptionId { get; }
        public Guid BillId { get; }

        public CreateSubscriptionBillCommand(Guid subscriptionId, Guid billId)
        {
            SubscriptionId = subscriptionId;
            BillId = billId;
        }
    }
}