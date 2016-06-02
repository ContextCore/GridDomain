using System;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    public class CreateServiceSubscriptionCommand : Command
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;
        public Guid SubscriptionId;

        public CreateServiceSubscriptionCommand(Guid id) : base(id)
        {
        }
    }
}