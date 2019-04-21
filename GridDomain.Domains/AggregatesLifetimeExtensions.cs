using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public static class AggregatesLifetimeExtensions
    {
        public static Task<AggregateHealthReport> GetHealth<TAggregate>(this IAggregatesController controller, string id, TimeSpan? timeout = null)
        {
            return controller.GetHealth(id.AsAddressFor<TAggregate>(), timeout);
        }
    }
}