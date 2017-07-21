using System;
using GridDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;

namespace GridDomain.Tests.Unit.Aggregate_actor_lifetime
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
            var processManagerDependenciesFactory = new SoftwareProgrammingProcessManagerDependenciesFactory(Logger);
            processManagerDependenciesFactory.StateDependencyFactory.RecycleConfigurationCreator = () => new TestPersistentChildsRecycleConfiguration();

            return new DomainConfiguration(b => b.RegisterAggregate(balloonDependencyFactory),
                                           b => b.RegisterProcessManager(processManagerDependenciesFactory));
        }

        private class TestPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(1);
            public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(2);
        }
    }
}