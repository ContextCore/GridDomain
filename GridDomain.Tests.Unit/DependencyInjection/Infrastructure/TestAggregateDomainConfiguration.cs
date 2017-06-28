using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new TestAggregatesCommandHandler(new TestDependencyImplementation())));
        }
    }
}