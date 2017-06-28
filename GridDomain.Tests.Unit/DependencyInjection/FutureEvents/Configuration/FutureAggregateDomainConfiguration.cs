using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration {
    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new FutureAggregateDependenciesFactory());
        }
    }
}