using System;
using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Common;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessDependenciesFactory: DefaultProcessDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessDependenciesFactory()
             :base(new SoftwareProgrammingProcess(),
                 () => new SoftwareProgrammingProcess(),
                 () => new SoftwareProgrammingProcessStateFactory(),
                   new RecycleConfiguration(TimeSpan.FromMilliseconds(1000),TimeSpan.FromMilliseconds(1000)))
        {
        }
    }
}
