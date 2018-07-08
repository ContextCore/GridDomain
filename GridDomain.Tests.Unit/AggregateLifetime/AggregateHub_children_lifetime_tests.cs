using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.AggregateLifetime
{
    public class AggregateHub_children_lifetime_tests : PersistentHubChildrenLifetimeTest
    {
        public AggregateHub_children_lifetime_tests(ITestOutputHelper output)
            : base(new PersistentHubFixture(output,new AggregatePersistedHubInfrastructure())) {}

    }
}