using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessDependenciesFactory: DefaultProcessDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessDependenciesFactory()
             :base(new SoftwareProgrammingProcess(),
                 () => new SoftwareProgrammingProcess(),
                 () => new SoftwareProgrammingProcessStateFactory())
        {
        }
    }
}
