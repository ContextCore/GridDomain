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
        public ProgrammingSoftwareSagaTest_with_custom_routes(ITestOutputHelper output)
            : base(output, new CustomRoutesFixture()) {}

        private class CustomRoutesFixture : NodeTestFixture
        {
            public CustomRoutesFixture()
            {
                Add(new CustomRoutesSoftwareProgrammingSagaMap());
                var cfg =
                    new CustomContainerConfiguration(c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
                        c =>
                            c
                                .RegisterAggregate
                                <SagaStateAggregate<SoftwareProgrammingSagaData>,
                                    SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>());
                Add(cfg);
                Add(new SampleDomainContainerConfiguration());
            }
        }
    }
}