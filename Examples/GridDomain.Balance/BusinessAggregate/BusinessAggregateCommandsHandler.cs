using BusinessNews.Domain.OfferAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class BusinessAggregateCommandsHandler : AggregateCommandsHandler<Business>
    {
        public BusinessAggregateCommandsHandler() : base(null)
        {
            Map<OrderSubscriptionCommand>(command => command.BusinessId,
                                         (command, aggregate) => aggregate.OrderSubscription(command.SubscriptionId, command.OfferId));

            Map<CompleteBusinessSubscriptionOrderCommand>(c => c.BusinessId,
                                                         (cmd, agr) => agr.PurchaseSubscription(cmd.SubscriptionId));

            Map<RegisterNewBusinessCommand>(c => c.BusinessId,
                                           cmd => new Business(cmd.BusinessId, cmd.Name, FreeSubscription.ID, cmd.AccountId));
        }
    }
}