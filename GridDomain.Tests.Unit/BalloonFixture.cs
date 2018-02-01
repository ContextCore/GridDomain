using System;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class BalloonFixture : NodeTestFixture
    {
        private readonly BalloonDomainConfiguration _balloonDomainConfiguration;

        public BalloonFixture(ITestOutputHelper output, IQuartzConfig config = null):base(output)
        {
            this.EnableScheduling(config);
            _balloonDomainConfiguration = new BalloonDomainConfiguration();
            Add(_balloonDomainConfiguration);

        }
        
        public BalloonFixture EnableSnapshots(
            int keep = 1,
            TimeSpan? maxSaveFrequency = null,
            int saveOnEach = 1)
        {
            var dependencyFactory = _balloonDomainConfiguration.BalloonDependencyFactory;

            dependencyFactory.SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            var balloonAggregateFactory = new BalloonAggregateFactory();

            dependencyFactory.AggregateFactoryCreator = () => balloonAggregateFactory;
            dependencyFactory.SnapshotsFactoryCreator = () => balloonAggregateFactory;

            return this;
        }
    }
}