using System;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.EventSourced.SnapshotsPolicy;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class SoftwareProgrammingProcessManagerFixture : NodeTestFixture
    {
        protected readonly SoftwareProgrammingProcessDomainConfiguration ProcessConfiguration;

        public SoftwareProgrammingProcessManagerFixture(ITestOutputHelper output) : base(output)
        {
            ProcessConfiguration = new SoftwareProgrammingProcessDomainConfiguration();
            Add(new BalloonDomainConfiguration());
            Add(ProcessConfiguration);
        }

        public SoftwareProgrammingProcessManagerFixture InitSnapshots(int eventsToKeep = 1,
                                                                      TimeSpan? maxSaveFrequency = null,
                                                                      int saveOnEach = 1)
        {
            var processStateDependencyFactory = ProcessConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory
                                                                    .StateDependencies;
            processStateDependencyFactory.SnapshotPolicy = new SnapshotsPersistencePolicy(saveOnEach, maxSaveFrequency, eventsToKeep);
            return this;
        }

        public SoftwareProgrammingProcessManagerFixture InitFastRecycle(TimeSpan? maxInactiveTime = null, TimeSpan? clearPeriod = null)
        {
            ProcessConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory.RecycleConfiguration =
                new RecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(200),
                                         maxInactiveTime ?? TimeSpan.FromMilliseconds(50));
            return this;
        }
    }
}