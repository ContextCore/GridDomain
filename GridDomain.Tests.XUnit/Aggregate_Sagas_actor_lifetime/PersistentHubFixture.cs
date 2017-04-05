using System;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.XUnit.Aggregate_Sagas_actor_lifetime
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
            return
                new CustomContainerConfiguration(
                                                 c =>
                                                     c.Register(
                                                                new SagaConfiguration<SoftwareProgrammingProcess, SoftwareProgrammingState, SoftwareProgrammingSagaFactory>(SoftwareProgrammingProcess.Descriptor, null, null)),
                                                 c =>
                                                     c
                                                         .RegisterAggregate
                                                         <SagaStateAggregate<SoftwareProgrammingState>,
                                                             SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingState>>(),
                                                 c => c.RegisterAggregate<Balloon, BalloonCommandHandler>(),
                                                 c => c.RegisterType<IPersistentChildsRecycleConfiguration, TestPersistentChildsRecycleConfiguration>());
        }

        private class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
            public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(2);
        }
    }
}