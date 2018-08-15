using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;

namespace GridDomain.Tests.Acceptance.Scheduling {
    class TestSchedulingAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(AggregateDependencies.New(new TestAggregateCommandHandler()));
        }
    }
}