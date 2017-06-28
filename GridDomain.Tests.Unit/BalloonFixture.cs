using System;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;

namespace GridDomain.Tests.Unit
{
    public class BalloonFixture : NodeTestFixture
    {
        private readonly BalloonDomainConfiguration _balloonDomainConfiguration;

        public BalloonFixture()
        {
            this.ClearSheduledJobs();
            _balloonDomainConfiguration = new BalloonDomainConfiguration();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(_balloonDomainConfiguration);
            return base.CreateNodeSettings();
        }

        public BalloonFixture InitSampleAggregateSnapshots(
            int keep = 1,
            TimeSpan? maxSaveFrequency = null,
            int saveOnEach = 1)
        {
            _balloonDomainConfiguration.BalloonDependencyFactory.SnapshotPolicyCreator
                = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            return this;
        }
    }
}