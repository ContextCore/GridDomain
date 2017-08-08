using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.Unit.FutureEvents.Configuration {
    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            var factory = new DefaultAggregateDependencyFactory<TestFutureEventsAggregate>(() => new FutureEventsAggregatesCommandHandler());
            builder.RegisterAggregate(factory);
        }
    }
}