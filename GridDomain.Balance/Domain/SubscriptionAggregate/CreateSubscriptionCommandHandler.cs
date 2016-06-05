using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class CreateSubscriptionCommandHandler : AggregateCommandsHandler<Subscription>
    {
        public CreateSubscriptionCommandHandler()
        {
            Map<CreateSubscriptionCommand>(cmd => cmd.SubscriptionId,
                cmd => new Subscription(cmd.SubscriptionId, cmd.Offer));
        }
    }
}