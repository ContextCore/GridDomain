using System;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers;
using Serilog;
using Serilog.Events;
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
            var dependencyFactory = _balloonDomainConfiguration.BalloonDependencies;

            dependencyFactory.SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, maxSaveFrequency, keep);
            var balloonAggregateFactory = new BalloonAggregateFactory();

            dependencyFactory.AggregateFactoryCreator = () => balloonAggregateFactory;
            dependencyFactory.SnapshotsFactoryCreator = () => balloonAggregateFactory;

            return this;
        }
        
        public BalloonFixture InitFastRecycle(
            TimeSpan? clearPeriod = null,
            TimeSpan? maxInactiveTime = null)
        {
            this._balloonDomainConfiguration.BalloonDependencies.RecycleConfigurationCreator = () =>
                                                                                                           new RecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(100),
                                                                                                                                         maxInactiveTime ?? TimeSpan.FromMilliseconds(200));
            return this;
        }
    }
}