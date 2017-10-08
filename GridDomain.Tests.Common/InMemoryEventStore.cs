using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Common {
    class InMemoryEventStore : IEventStore
    {
        public Task Persist(IReadOnlyCollection<DomainEvent> events)
        {
            _events.AddRange(events);
            return Task.CompletedTask;
        }

        public Task Persist(IAggregate aggregate)
        {
            return Persist(aggregate.GetUncommittedEvents());
        }

        private List<DomainEvent> _events = new List<DomainEvent>();
        public IReadOnlyCollection<DomainEvent> Events => _events;

        public void Clear()
        {
            _events.Clear();
        }
    }
}