using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tools
{
    public static class AggregateExtensions
    {
        public static void PersistAll(this Aggregate aggregate)
        {
            foreach (var e in ((IAggregate) aggregate).GetUncommittedEvents().ToArray())
                aggregate.MarkPersisted(e);
        }
    }
}