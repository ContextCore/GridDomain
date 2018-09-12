using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders;

namespace GridDomain.Tests.Unit.EventsUpgrade {
    public class BalanceDomainDonfiguration : IDomainConfiguration
    {
        public AggregateDependencies<BalanceAggregate> AggregateDependencies;

        public BalanceDomainDonfiguration()
        {
            AggregateDependencies = Configuration.AggregateDependencies.New<BalanceAggregate>(null);

        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(AggregateDependencies);
            builder.RegisterHandler<BalanceChangedEvent_V0, SampleProjectionBuilder>(c => new SampleProjectionBuilder(c.Publisher)).AsSync();
            builder.RegisterHandler<BalanceChangedEvent_V1, SampleProjectionBuilder>(c => new SampleProjectionBuilder(c.Publisher)).AsSync();
        }
    }
}