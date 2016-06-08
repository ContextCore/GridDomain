using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessNews.Domain.OfferAggregate
{
    public static class WellKnownOffers
    {
        public static readonly IDictionary<Guid, Offer> Catalog = new Dictionary<Guid, Offer>();
        
        static WellKnownOffers()
        {
            var free = new FreeSubscription();
            var paid = new VIPSubscription();
            
            Catalog[free.Id] = free;
            Catalog[paid.Id] = paid;
        }
    }
}