using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessNews.Domain.OfferAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.SubscriptionAggregate
{
    class SubscriptionAggregateCommandsHandler: AggregateCommandsHandler<Subscription>
    {
        public SubscriptionAggregateCommandsHandler()
        {
            Map<CreateSubscriptionCommand>(c => c.SubscriptionId,
                                           c => new Subscription(c.SubscriptionId, WellKnownOffers.Catalog[c.Offer]));

            Map<ChargeSubscriptionCommand>(c => c.SubscriptionId,
                                           (c,a) => a.Charge(a.Id));
        }
    }
}
