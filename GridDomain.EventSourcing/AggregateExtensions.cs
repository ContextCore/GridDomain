using System.Linq;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public static class AggregateExtensions
    {
        public static DomainEvent[] GetDomainEvents(this IAggregate aggregate)
        {
            return aggregate.GetUncommittedEvents()
                            .Cast<DomainEvent>()
                            .ToArray();
        }

        public static void PersistAll(this Aggregate aggregate)
        {
            foreach (var e in aggregate.GetDomainEvents())
                aggregate.MarkPersisted(e);
        }
    }
}