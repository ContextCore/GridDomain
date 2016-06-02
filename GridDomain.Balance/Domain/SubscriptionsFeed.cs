using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.OfferAggregate;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class SubscriptionsFeed
    {
        public IEnumerable<ICommand> InitialSubscriptions()
        {
            yield return new CreateServiceSubscriptionCommand(Guid.Parse("26F3E274-F486-4B78-A3DA-8ECD321FDFFE"))
            {
                Cost = Money.Zero(Currency.Rub),
                Grants = new[] {"dashboard", "social_networks"},
                Name = "Free",
                Period = TimeSpan.MaxValue,
                SubscriptionId = Guid.Parse("E70D9675-0A64-4081-87C4-A4E13548717B")
            };

            yield return new CreateServiceSubscriptionCommand(Guid.Parse("4828C630-42A3-4EDF-8835-763F98C80C3D"))
            {
                Cost = new Money(100, Currency.Rub),
                Grants = new[] {"dashboard", "social_networks", "web_site", "shop"},
                Name = "Paid",
                Period = TimeSpan.FromDays(30),
                SubscriptionId = Guid.Parse("E9721F21-5549-4983-8C45-672BF15C09B8")
            };
        }
    }
}