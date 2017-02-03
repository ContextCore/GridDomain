using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.Sagas.CustomRoutesSoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class ProgrammingSoftwareSagaTest_with_custom_routes : NodeTestKit
    {
        class CustomRoutesFixture : NodeTestFixture
        {
            protected override IMessageRouteMap CreateRouteMap()
            {
                return new CustomRoutesSoftwareProgrammingSagaMap();
            }

            protected override IContainerConfiguration CreateContainerConfiguration()
            {
                return new CustomContainerConfiguration(
                    c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
                    c => c.Register(new SampleDomainContainerConfiguration()),
                    c => c.RegisterAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>,
                                             SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                    );
            }
        }

        public ProgrammingSoftwareSagaTest_with_custom_routes(ITestOutputHelper output) : base(output,new CustomRoutesFixture()) {}
    }
}