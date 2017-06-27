using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration
{
    public class SoftwareProgrammingSagaDependenciesFactory: DefaultSagaDependencyFactory<SoftwareProgrammingProcess, SoftwareProgrammingState>
    {
        public SoftwareProgrammingSagaDependenciesFactory(ILogger log):base(new SoftwareProgrammingSagaFactory(log), SoftwareProgrammingProcess.Descriptor)
        {
            
        }
    }
}
