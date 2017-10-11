using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessManagerDependenciesFactory: DefaultProcessDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory():base(new SoftwareProgrammingProcess())
        {
            ProcessFactory = () => new SoftwareProgrammingProcess();
            ProcessStateFactory = () => new SoftwareProgrammingProcessStateFactory();
        }
    }
}
