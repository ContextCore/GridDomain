using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain {
    public class HomeDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new HomeAggregateHandler()));
        }
    }
}