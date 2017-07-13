using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration {
    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new FutureAggregateDependenciesFactory());
        }
    }
}