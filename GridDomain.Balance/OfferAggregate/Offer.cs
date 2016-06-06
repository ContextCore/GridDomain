using System;
using System.Collections.Generic;
using System.Threading;
using NMoneys;

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

    //for demo purposes offers is predefined 
    public abstract class Offer
    {
        public Guid Id { get; protected set; }
        public Money Price { get; protected set; }
        public string[] Grants { get; protected set; }
        public string Name { get; protected set; }
        public TimeSpan Period { get; protected set; }
    }
}