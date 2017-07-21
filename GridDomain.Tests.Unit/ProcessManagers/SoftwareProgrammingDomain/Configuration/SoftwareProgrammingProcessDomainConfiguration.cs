using GridDomain.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingProcessDomainConfiguration : IDomainConfiguration
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory SoftwareProgrammingProcessManagerDependenciesFactory { get; }

        public SoftwareProgrammingProcessDomainConfiguration(ILogger log)
        {
            SoftwareProgrammingProcessManagerDependenciesFactory = new SoftwareProgrammingProcessManagerDependenciesFactory(log);
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterProcessManager(SoftwareProgrammingProcessManagerDependenciesFactory);
        }
    }
}