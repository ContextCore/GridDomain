using System;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Actors.EventSourced;
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
            ProcessConfiguration = new SoftwareProgrammingProcessDomainConfiguration(Logger);
            Add(new BalloonDomainConfiguration());
            Add(ProcessConfiguration);

        }
        
        public SoftwareProgrammingProcessManagerFixture InitSnapshots(int keep = 1,
                                                            TimeSpan? maxSaveFrequency = null,
                                                            int saveOnEach = 1)
        {
            var processStateDependencyFactory = ProcessConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory
                                                                    .StateDependencyFactory;
            processStateDependencyFactory.SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
          //  processStateDependencyFactory.SnapshotsFactoryCreator = () => 
            return this;
        }
    }
}