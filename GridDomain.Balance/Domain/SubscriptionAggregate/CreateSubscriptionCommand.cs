using System;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class CreateSubscriptionCommand : Command
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;
        public Guid SubscriptionId;

        public CreateSubscriptionCommand(Guid id) : base(id)
        {
        }
    }
}