using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaContainerConfiguration : IContainerConfiguration
    {
        private readonly IContainerConfiguration _sagaConfiguration =
            new SagaConfiguration<SoftwareProgrammingProcess, SoftwareProgrammingState, SoftwareProgrammingSagaFactory>(SoftwareProgrammingProcess.Descriptor, null, null);

        public void Register(IUnityContainer container)
        {
            _sagaConfiguration.Register(container);
        }
    }
}