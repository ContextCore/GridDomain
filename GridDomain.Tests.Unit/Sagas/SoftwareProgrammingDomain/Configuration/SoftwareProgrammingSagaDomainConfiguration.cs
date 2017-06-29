using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingSagaDomainConfiguration : IDomainConfiguration
    {
        public SoftwareProgrammingSagaDependenciesFactory SoftwareProgrammingSagaDependenciesFactory { get; }

        public SoftwareProgrammingSagaDomainConfiguration(ILogger log)
        {
            SoftwareProgrammingSagaDependenciesFactory = new SoftwareProgrammingSagaDependenciesFactory(log);
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterSaga(SoftwareProgrammingSagaDependenciesFactory);
        }
    }
}