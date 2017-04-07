using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;

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