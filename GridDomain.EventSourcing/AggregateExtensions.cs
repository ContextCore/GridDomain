using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public static class AggregateExtensions
    {
        public static IReadOnlyCollection<DomainEvent> GetDomainEvents(this IAggregate aggregate)
        {
            return aggregate.GetUncommittedEvents();
        }

        public static void PersistAll(this Aggregate aggregate)
        {
            foreach (var e in aggregate.GetDomainEvents().ToArray())
                aggregate.MarkPersisted(e);
        }
    }
}