using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public interface IEventStore
    {
        Task Persist(IAggregate aggregate);
    }
}