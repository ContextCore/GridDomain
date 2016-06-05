using System;
using NMoneys;

namespace BusinessNews.Domain.OfferAggregate
{
    public class VIPSubscription : Offer
    {
        public static readonly Guid ID = Guid.Parse("BFE2B6DD-3256-4580-9D64-9BCECD90EB26");

        public VIPSubscription()
        {
            Id = ID;
            Name = "Vip subscription";
            Price = new Money(10);
            Grants = new[] { "free", "vip" };
            Period = TimeSpan.FromDays(30);
        }

    }
}