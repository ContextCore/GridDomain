using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration {
    public class SoftwareProgrammingSagaDomainConfiguration : IDomainConfiguration
    {
        private readonly ILogger _logger;

        public SoftwareProgrammingSagaDomainConfiguration(ILogger log)
        {
            _logger = log;
        }
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterSaga(new SoftwareProgrammingSagaDependenciesFactory(_logger));
        }
    }
}