using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingSagaDomainConfiguration : IDomainConfiguration
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory SoftwareProgrammingProcessManagerDependenciesFactory { get; }

        public SoftwareProgrammingSagaDomainConfiguration(ILogger log)
        {
            SoftwareProgrammingProcessManagerDependenciesFactory = new SoftwareProgrammingProcessManagerDependenciesFactory(log);
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterProcessManager(SoftwareProgrammingProcessManagerDependenciesFactory);
        }
    }
}