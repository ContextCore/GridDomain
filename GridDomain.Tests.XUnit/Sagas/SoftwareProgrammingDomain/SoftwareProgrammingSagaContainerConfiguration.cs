using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingSagaContainerConfiguration : IContainerConfiguration
    {
        private readonly IContainerConfiguration _sagaConfiguration =
            SagaConfiguration.Instance<SoftwareProgrammingSaga, SoftwareProgrammingSagaState, SoftwareProgrammingSagaFactory>(
                                                                                                                             SoftwareProgrammingSaga.Descriptor);

        public void Register(IUnityContainer container)
        {
            _sagaConfiguration.Register(container);
        }
    }
}