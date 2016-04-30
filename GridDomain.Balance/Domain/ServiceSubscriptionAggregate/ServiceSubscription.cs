using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using Itenso.TimePeriod;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class ServiceSubscription : AggregateBase
    {
        public TimeSpan Period;
        public Money Cost;
        public SubscriptionRights Rights;

        private ServiceSubscription(Guid id)
        {
            Id = id;
        }

        public ServiceSubscription(Guid id, TimeSpan period, Money cost, SubscriptionRights rights):this(id)
        {
            RaiseEvent(new ServiceSubscriptionCreatedEvent(id)
            {
                Period = period, Cost = cost, Rights = rights
            });
        }

        private void Apply(ServiceSubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Period = e.Period;
            Cost = e.Cost;
            Rights = e.Rights;
        }
    }
}
