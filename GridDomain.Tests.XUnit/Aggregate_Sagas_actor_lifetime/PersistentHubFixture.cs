using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.SampleDomain;
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

        private IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => c.Register(
                    SagaConfiguration.Instance<SoftwareProgrammingSaga,
                        SoftwareProgrammingSagaData,
                        SoftwareProgrammingSagaFactory>(
                            SoftwareProgrammingSaga.Descriptor)),

                c => c.RegisterAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>,
                    SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>(),

                c => c.RegisterAggregate<SampleAggregate, SampleAggregatesCommandHandler>(),

                c => c.RegisterType<IPersistentChildsRecycleConfiguration, TestPersistentChildsRecycleConfiguration>()
                );
        }

        public IPersistentActorTestsInfrastructure Infrastructure { get; }

        class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
            public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(2);
        }
    }
}