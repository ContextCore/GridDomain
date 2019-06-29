using GridDomain.Abstractions;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Domains;

namespace GridDomain.Node.Akka
{
    public static class DomainBuilderExtensions
    {
        public static IAggregatesDomainBuilder GetAggregatesBuilder(this IDomainBuilder builder)
        {
            return builder.GetPart<IAggregatesDomainBuilder>();
        }
    }
}