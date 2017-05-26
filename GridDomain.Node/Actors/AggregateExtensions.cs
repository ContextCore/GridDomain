using System.Linq;

using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Node.Actors
{
    static class AggregateExtensions
    {
        public static DomainEvent[] GetDomainEvents(this IAggregate aggregate)
        {
            return aggregate.GetUncommittedEvents()
                            .Cast<DomainEvent>()
                            .ToArray();
        }
    }
}