using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public static class AggregateExtensions
    {
        public static void MarkAllPesisted(this IAggregate aggregate)
        {
            foreach (var e in aggregate.GetUncommittedEvents())
                aggregate.ApplyEvent(e);

            aggregate.ClearUncommitedEvents();
        }
    }
}