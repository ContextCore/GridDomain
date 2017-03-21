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
        public SoftwareProgrammingSagaFixture(IContainerConfiguration config = null,
                                              IMessageRouteMap map = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, map, timeout)
        {
            Add(new CustomContainerConfiguration(c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
                                                 c =>
                                                     c
                                                         .RegisterAggregate
                                                         <SagaStateAggregate<SoftwareProgrammingSagaState>,
                                                             SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaState>>()));
            Add(new SoftwareProgrammingSagaRoutes());
            Add(new SampleDomainContainerConfiguration());
        }
    }
}