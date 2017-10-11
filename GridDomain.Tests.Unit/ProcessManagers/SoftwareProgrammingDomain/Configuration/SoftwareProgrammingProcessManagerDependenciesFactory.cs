using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessManagerDependenciesFactory: DefaultProcessManagerDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory(ILogger log):base(new SoftwareProgrammingProcessStateFactory(log), SoftwareProgrammingProcess.Descriptor)
        {
            
        }
    }
}
