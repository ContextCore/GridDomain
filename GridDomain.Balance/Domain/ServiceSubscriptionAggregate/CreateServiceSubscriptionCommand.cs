using System;
using GridDomain.CQRS;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    public class CreateServiceSubscriptionCommand : Command
    {
        public TimeSpan Period;
        public Money Cost;
        public string[] Grants;
        public string Name;
        public Guid SubscriptionId;

        public CreateServiceSubscriptionCommand(Guid id):base(id)
        {
            
        }
    }
}