using System;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration;
using Microsoft.Practices.Unity;
using Serilog;

namespace GridDomain.Tests.Unit.Aggregate_Sagas_actor_lifetime
{
    public class PersistentHubFixture : NodeTestFixture
    {
        public PersistentHubFixture(IPersistentActorTestsInfrastructure infrastructure) : base()
        {
            Infrastructure = infrastructure;
            Add(CreateDomainConfiguration());
        }

        public IPersistentActorTestsInfrastructure Infrastructure { get; }

        private IDomainConfiguration CreateDomainConfiguration()
        {
            var balloonDependencyFactory = new BalloonDependencyFactory() {RecycleConfigurationCreator = () => new TestPersistentChildsRecycleConfiguration()};
            var sagaDependencyFactory = new SoftwareProgrammingSagaDependenciesFactory(Logger);
            sagaDependencyFactory.StateDependencyFactory.RecycleConfigurationCreator = () => new TestPersistentChildsRecycleConfiguration();

            return new DomainConfiguration(b => b.RegisterAggregate(balloonDependencyFactory),
                                           b => b.RegisterSaga(sagaDependencyFactory));
        }

        private class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
            public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(2);
        }
    }
}