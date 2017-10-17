using GridDomain.Configuration;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingProcessDomainConfiguration : IDomainConfiguration
    {
        public DefaultProcessDependencyFactory<SoftwareProgrammingState> SoftwareProgrammingProcessManagerDependenciesFactory { get;  set; }

        public SoftwareProgrammingProcessDomainConfiguration(ILogger log)
        {
            SoftwareProgrammingProcessManagerDependenciesFactory = new SoftwareProgrammingProcessDependenciesFactory();
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterProcessManager(SoftwareProgrammingProcessManagerDependenciesFactory);
            new SoftwareDomainConfiguration().Register(builder);
        }
    }
}