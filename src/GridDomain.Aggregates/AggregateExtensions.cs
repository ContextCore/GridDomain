using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Aggregates
{
    public static class AggregateExtensions
    {
        public static void ApplyByVersion(this IAggregate aggregate, params IDomainEvent[] evt)
        {
            foreach (var ev in evt)
                if (aggregate.Version == ev.Version)
                    aggregate.Apply(ev);
        }
    }
}