using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            var aggregateDependencyFactory = AggregateDependencies.New<TestAggregate>(null);
            aggregateDependencyFactory.AggregateFactory =  new TestAggregateFactory(new TestDependencyImplementation());
            aggregateDependencyFactory.SnapshotFactory =  new TestAggregateFactory(new TestDependencyImplementation());
            builder.RegisterAggregate(aggregateDependencyFactory);
        }
    }
}