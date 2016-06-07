using System;
using System.Collections.Generic;

namespace BusinessNews.Domain.OfferAggregate
{
    public static class WellKnownOffers
    {
        public static IDictionary<Guid, Offer> Catalog = new Dictionary<Guid, Offer>();

        static WellKnownOffers()
        {
            var free = new FreeSubscription();
            var paid = new VIPSubscription();

            Catalog[free.Id] = free;
            Catalog[paid.Id] = paid;
        }
    }
}