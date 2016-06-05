using System;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{

    public class FreeSubscription : Offer
    {
        public static readonly Guid ID = Guid.Parse("9EA47C4B-7605-45B6-AD21-C552CE8BD211");

        public FreeSubscription()
        {
            Id = ID;
            Name = "Free subscription";
            Cost = Money.Zero();
            Grants = new[] {"free"};
            Period = TimeSpan.MaxValue;
        }
    }

    public class VIPSubscription : Offer
    {
        public static readonly Guid ID = Guid.Parse("BFE2B6DD-3256-4580-9D64-9BCECD90EB26");

        public VIPSubscription()
        {
            Id = ID;
            Name = "Vip subscription";
            Cost = new Money(10);
            Grants = new[] { "free", "vip" };
            Period = TimeSpan.FromDays(30);
        }

    }

    //for demo purposes offers is predefined 
    public abstract class Offer
    {
        public Guid Id { get; protected set; }
        public Money Cost { get; protected set; }
        public string[] Grants { get; protected set; }
        public string Name { get; protected set; }
        public TimeSpan Period { get; protected set; }
    }
}