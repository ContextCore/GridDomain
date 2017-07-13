using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;

namespace GridDomain.Tests.Unit.EventsUpgrade {
    public class BalanceDomainDonfiguration : IDomainConfiguration
    {
        public DefaultAggregateDependencyFactory<BalanceAggregate> DefaultAggregateDependencyFactory;

        public BalanceDomainDonfiguration()
        {
            DefaultAggregateDependencyFactory = Node.Configuration.Composition.DefaultAggregateDependencyFactory.New(new BalanceAggregatesCommandHandler(), new BalanceRouteMap());

        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory);
        }
    }
}