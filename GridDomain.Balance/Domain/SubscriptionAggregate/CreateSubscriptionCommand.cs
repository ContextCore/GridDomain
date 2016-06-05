using System;
using System.Security.Principal;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class CreateSubscriptionCommand : Command
    {
        public CreateSubscriptionCommand(Guid offer, Guid subscriptionId)
        {
            Offer = offer;
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
        public Guid Offer { get; }
    }

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