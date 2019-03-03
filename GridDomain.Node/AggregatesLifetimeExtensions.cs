using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node
{
    public static class AggregatesLifetimeExtensions
    {
        public static Task<AggregateHealthReport> GetHealth<TAggregate>(this IAggregatesLifetime lifetime, string id, TimeSpan? timeout = null)
        {
            return lifetime.GetHealth(id.AsAddressFor<TAggregate>(), timeout);
        }
    }
}