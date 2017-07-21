using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingProcessManagerDependenciesFactory: DefaultProcessManagerDependencyFactory<SoftwareProgrammingState>
    {
        public SoftwareProgrammingProcessManagerDependenciesFactory(ILogger log):base(new SoftwareProgrammingProcessManagerFactory(log), SoftwareProgrammingProcess.Descriptor)
        {
            
        }
    }
}
