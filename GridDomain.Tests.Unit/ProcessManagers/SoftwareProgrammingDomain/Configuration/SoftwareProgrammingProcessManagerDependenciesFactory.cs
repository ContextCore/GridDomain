using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessManagerDependenciesFactory: DefaultProcessManagerDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory(ILogger log):base(new SoftwareProgrammingProcessManagerFactory(log), SoftwareProgrammingProcess.Descriptor)
        {
            
        }
    }
}
