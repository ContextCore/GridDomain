using System;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers;

namespace GridDomain.Tests.Unit
{
    public class BalloonFixture : NodeTestFixture
    {
        private readonly BalloonDomainConfiguration _balloonDomainConfiguration;

        public BalloonFixture(IQuartzConfig config = null)
        {
            this.EnableScheduling(config);
            _balloonDomainConfiguration = new BalloonDomainConfiguration();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(_balloonDomainConfiguration);
            return base.CreateNodeSettings();
        }

        public BalloonFixture EnableSnapshots(
            int keep = 1,
            TimeSpan? maxSaveFrequency = null,
            int saveOnEach = 1)
        {
            var dependencyFactory = _balloonDomainConfiguration.BalloonDependencyFactory;

            dependencyFactory.SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            dependencyFactory.AggregateFactoryCreator = () => new BalloonAggregateFactory();

            return this;
        }
    }
}