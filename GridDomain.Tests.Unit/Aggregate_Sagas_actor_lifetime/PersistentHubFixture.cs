using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{
    public class PersistentHubFixture : NodeTestFixture
    {
        public PersistentHubFixture(IPersistentActorTestsInfrastructure infrastructure)
        {
            Infrastructure = infrastructure;
            Add(CreateConfiguration());
            Add(new SoftwareProgrammingSagaRoutes());
        }

        public IPersistentActorTestsInfrastructure Infrastructure { get; }

        private IContainerConfiguration CreateConfiguration()
        {
            return new ContainerConfiguration(
                                              c => c.Register(SagaConfiguration.New<SoftwareProgrammingProcess, SoftwareProgrammingState, SoftwareProgrammingSagaFactory>
                                                                  (SoftwareProgrammingProcess.Descriptor)),
                                              c => { c.Register(AggregateConfiguration.New<SagaStateAggregate<SoftwareProgrammingState>, SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingState>>()); },
                                              c => { c.Register(AggregateConfiguration.New<Balloon, BalloonCommandHandler>()); },
                                              c => c.RegisterType<IPersistentChildsRecycleConfiguration, TestPersistentChildsRecycleConfiguration>());
        }

        private class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
            public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(2);
        }
    }
}