using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    class FakeEventStore : IEventStore
    {
        public Task Persist(IReadOnlyCollection<DomainEvent> events)
        {
            return Task.CompletedTask;
        }

        public Task Persist(IAggregate aggregate)
        {
            return Task.CompletedTask;
        }
    }
}