using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;

namespace GridDomain.Tests.Unit.EventsUpgrade {
    public class BalanceDomainDonfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new BalanceAggregatesCommandHandler(), new BalanceRouteMap()));
        }
    }
}