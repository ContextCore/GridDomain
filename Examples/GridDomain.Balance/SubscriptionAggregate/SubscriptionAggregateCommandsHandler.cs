using BusinessNews.Domain.OfferAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    public class SubscriptionAggregateCommandsHandler : AggregateCommandsHandler<Subscription>
    {
        public SubscriptionAggregateCommandsHandler() : base(null)
        {
            Map<CreateSubscriptionCommand>(c => c.SubscriptionId,
                c => new Subscription(c.SubscriptionId, WellKnownOffers.Catalog[c.Offer]));

            Map<ChargeSubscriptionCommand>(c => c.SubscriptionId,
                (c, a) => a.Charge(a.Id));
        }
    }
}