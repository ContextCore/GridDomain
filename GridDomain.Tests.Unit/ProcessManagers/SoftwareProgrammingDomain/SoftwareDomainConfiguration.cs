using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain {
    public class SoftwareDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.ForCommandAggregate<ProgrammerAggregate>());
        }
    }
}