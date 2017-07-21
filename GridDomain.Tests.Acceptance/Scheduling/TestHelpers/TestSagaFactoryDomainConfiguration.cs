using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers {
    public class TestSagaFactoryDomainConfiguration : IDomainConfiguration
    {
        private readonly ILogger _logger;
        public TestSagaFactoryDomainConfiguration(ILogger logger)
        {
            _logger = logger;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterProcessManager(new DefaultProcessManagerDependencyFactory<TestProcessState>(new TestProcessManagerFactory(_logger),TestSaga.Descriptor));
        }

     
    }
}