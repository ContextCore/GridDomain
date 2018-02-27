using GridDomain.Configuration;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingProcessDomainConfiguration : IDomainConfiguration
    {
        public DefaultProcessDependencyFactory<SoftwareProgrammingState> SoftwareProgrammingProcessManagerDependenciesFactory { get;  set; }

        public SoftwareProgrammingProcessDomainConfiguration()
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