using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Common {
    class InMemoryEventStore : IEventStore
    {
        public Task Persist(IAggregate aggregate)
        {
            _events.AddRange(aggregate.GetUncommittedEvents());
            return Task.CompletedTask;
        }

        private readonly List<DomainEvent> _events = new List<DomainEvent>();
        public IReadOnlyCollection<DomainEvent> Events => _events;

        public void Clear()
        {
            _events.Clear();
        }
    }
}