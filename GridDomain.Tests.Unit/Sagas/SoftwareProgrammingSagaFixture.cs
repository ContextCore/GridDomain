using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;

namespace GridDomain.Tests.Unit.Sagas
{
    public class SoftwareProgrammingSagaFixture : NodeTestFixture
    {
        public SoftwareProgrammingSagaFixture(IDomainBuilderConfiguration config = null,
                                              IMessageRouteMap map = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, map, timeout)
        {
            var cfg = new CustomContainerConfiguration(c => c.Register(SagaConfiguration.New<SoftwareProgrammingProcess,
                                                                           SoftwareProgrammingState,
                                                                           SoftwareProgrammingSagaFactory>(SoftwareProgrammingProcess.Descriptor)),
                                                       c => c.RegisterAggregate<SagaStateAggregate<SoftwareProgrammingState>,
                                                           SagaStateCommandHandler<SoftwareProgrammingState>>());
            Add(cfg);
            Add(new SoftwareProgrammingSagaRoutes());
            Add(new BalloonDomainConfiguration());
        }
    }
}