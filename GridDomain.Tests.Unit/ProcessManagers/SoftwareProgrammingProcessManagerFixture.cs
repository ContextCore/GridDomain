using System;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class SoftwareProgrammingProcessManagerFixture : NodeTestFixture
    {
        protected readonly SoftwareProgrammingProcessDomainConfiguration ProcessConfiguration;

        public SoftwareProgrammingProcessManagerFixture(IDomainConfiguration config = null,
                                                        TimeSpan? timeout = default(TimeSpan?)) : base(config, timeout)
        {
            ProcessConfiguration = new SoftwareProgrammingProcessDomainConfiguration(Logger);
        //    Add(new BalloonDomainConfiguration());
            Add(ProcessConfiguration);

        }
        
        public SoftwareProgrammingProcessManagerFixture InitSnapshots(int keep = 1,
                                                            TimeSpan? maxSaveFrequency = null,
                                                            int saveOnEach = 1)
        {
            ProcessConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory
                                                       .StateDependencyFactory
                                                       .SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            return this;
        }
    }
}