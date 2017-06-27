using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration
{
    class SoftwareProgrammingDependencyFactory
    {
        ISagaDependencyFactory<SoftwareProgrammingProcess, SoftwareProgrammingState> New(SoftwareProgrammingSagaFactory factory)
        {
            return SagaDependencyFactory.FromSagaCreator<SoftwareProgrammingProcess, SoftwareProgrammingState>(factory, SoftwareProgrammingProcess.Descriptor);
        }
    }
}
