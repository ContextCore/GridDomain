using System;
using BusinessNews.Domain.AccountAggregate;
using BusinessNews.Domain.OfferAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class BusinessAggregateCommandsHandler : AggregateCommandsHandler<Business>
    {
        public BusinessAggregateCommandsHandler()
        {
            Map<OrderSubscriptionCommand>(c => c.BusinessId,
                (c, a) => a.OrderSubscription(c.SubscriptionId, c.OfferId));

            Map<CompleteBusinessSubscriptionOrderCommand>(c => c.BusinessId,
                (cmd, agr) => agr.PurchaseSubscription(cmd.SubscriptionId));

            Map<RegisterNewBusinessCommand>(c => c.BusinessId,
                                           cmd => new Business(cmd.BusinessId, cmd.Name, FreeSubscription.ID, cmd.AccountId));
        }
    }
}