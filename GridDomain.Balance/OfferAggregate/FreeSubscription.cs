using System;
using NMoneys;

namespace BusinessNews.Domain.OfferAggregate
{
    public class FreeSubscription : Offer
    {
        public static readonly Guid ID = Guid.Parse("9EA47C4B-7605-45B6-AD21-C552CE8BD211");

        public FreeSubscription()
        {
            Id = ID;
            Name = "Free subscription";
            Price = Money.Zero();
            Grants = new[] { WellKnownGrants.PublicNewsRead};
            Period = TimeSpan.MaxValue;
        }
    }
}