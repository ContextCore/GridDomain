using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Itenso.TimePeriod;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class SubscriptionRights
    {
        public Guid Id { get; }
        public string Name { get; }
        public Grant[] Rights { get; }
    }

    class Grant
    {
        public string Value;
    }

    class ServiceSubscription
    {
        public Guid Id;
        public TimeSpan Period;
        public Money Cost;
        public SubscriptionRights Rights;
    }
}
