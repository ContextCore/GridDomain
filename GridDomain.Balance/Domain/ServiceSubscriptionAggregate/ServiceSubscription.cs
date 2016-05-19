using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using Itenso.TimePeriod;
using Microsoft.SqlServer.Server;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class ServiceSubscription : AggregateBase
    {
        public TimeSpan Period;
        public Money Cost;
        public string[] Grants;
        public string Name;

        private ServiceSubscription(Guid id)
        {
            Id = id;
        }

        public ServiceSubscription(Guid id, TimeSpan period, Money cost, string name, string[] grants):this(id)
        {
            RaiseEvent(new ServiceSubscriptionCreatedEvent(id)
            {
                Period = period, Cost = cost, Name = name,Grants = grants
            });
        }

        private void Apply(ServiceSubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Period = e.Period;
            Cost = e.Cost;
            Grants = e.Grants;
            Name = e.Name;
        }
    }
}
