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
        protected override IMessageRouteMap RouteMap { get; } = new CustomRoutesSoftwareProgrammingSagaMap();

        protected override IContainerConfiguration ContainerConfiguration => 
            new SagaConfiguration<CustomRoutesSoftwareProgrammingSaga,
                                  SoftwareProgrammingSagaData,
                                  CustomRoutesSoftwareProgrammingSagaFactory>(CustomRoutesSoftwareProgrammingSaga.Descriptor);
    }
}