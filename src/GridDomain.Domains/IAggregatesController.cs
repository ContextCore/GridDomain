using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public interface IAggregatesController
    {
        //  Task WakeUp(string aggregateId);
        //  Task Shutdown(Type aggregateType, string aggregateId);
        //  Task<bool> IsActive(Type type, string catName, TimeSpan? timeout =null);
        Task<AggregateHealthReport> GetHealth(IAggregateAddress address, TimeSpan? timeout = null);
    }
}