using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
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