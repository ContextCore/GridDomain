using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Infrastructure;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration {
    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            var factory = new DefaultAggregateDependencyFactory<TestFutureEventsAggregate>(() => new FutureEventsAggregatesCommandHandler());
            builder.RegisterAggregate(factory);
        }
    }
}