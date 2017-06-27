using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.Sagas
{
    public class SoftwareProgrammingSagaFixture : NodeTestFixture
    {
        public SoftwareProgrammingSagaFixture(IDomainConfiguration config = null,
                                              IMessageRouteMap map = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, map, timeout)
        {
            var cfg = new ContainerConfiguration(c => c.Register(SagaConfiguration.New<SoftwareProgrammingProcess,
                                                                     SoftwareProgrammingState,
                                                                     SoftwareProgrammingSagaFactory>(SoftwareProgrammingProcess.Descriptor,
                                                                                                     () => c.Resolve<SoftwareProgrammingSagaFactory>())),
                                                 c => { c.Register(AggregateConfiguration.New<SagaStateAggregate<SoftwareProgrammingState>, SagaStateCommandHandler<SoftwareProgrammingState>>()); });
            Add(cfg);
            Add(new SoftwareProgrammingSagaRoutes());
            Add(new BalloonDomainConfiguration());
        }
    }
}