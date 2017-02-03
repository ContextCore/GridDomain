using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.SampleDomain;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class SoftwareProgrammingSagaFixture : NodeTestFixture
    {
        public SoftwareProgrammingSagaFixture(IContainerConfiguration config = null, IMessageRouteMap map = null, TimeSpan? timeout = default(TimeSpan?)) : base(config, map, timeout)
        {
        }

        protected override IContainerConfiguration CreateContainerConfiguration()
        {
            var baseConf = new SampleDomainContainerConfiguration();
            return new CustomContainerConfiguration(
                c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
                c => c.Register(baseConf),
                c => c.RegisterAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>,
                    SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                );
        }

        protected override IMessageRouteMap CreateRouteMap()
        {
            return  new SoftwareProgrammingSagaRoutes();
        }
    }
}