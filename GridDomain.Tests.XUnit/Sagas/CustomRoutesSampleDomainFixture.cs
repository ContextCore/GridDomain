using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class CustomRoutesSampleDomainFixture : NodeTestFixture
    {
        private readonly IMessageRouteMap _routeMap1 = new CustomRoutesSoftwareProgrammingSagaMap();

        protected override IMessageRouteMap CreateRouteMap()
        {
            return _routeMap1;
        }

        protected override IContainerConfiguration CreateContainerConfiguration()
            => new SagaConfiguration<CustomRoutesSoftwareProgrammingSaga,
                SoftwareProgrammingSagaData,
                CustomRoutesSoftwareProgrammingSagaFactory>(CustomRoutesSoftwareProgrammingSaga.Descriptor);
    }
}