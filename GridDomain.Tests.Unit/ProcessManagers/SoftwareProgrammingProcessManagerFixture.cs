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
        private readonly SoftwareProgrammingProcessDomainConfiguration _softwareProgrammingProcessDomainConfiguration;

        public SoftwareProgrammingProcessManagerFixture(IDomainConfiguration config = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, timeout)
        {
            _softwareProgrammingProcessDomainConfiguration = new SoftwareProgrammingProcessDomainConfiguration(Logger);
            Add(new BalloonDomainConfiguration());
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(_softwareProgrammingProcessDomainConfiguration);
            return base.CreateNodeSettings();
        }

        public SoftwareProgrammingProcessManagerFixture InitSnapshots(int keep = 1,
                                                            TimeSpan? maxSaveFrequency = null,
                                                            int saveOnEach = 1)
        {
            _softwareProgrammingProcessDomainConfiguration.SoftwareProgrammingProcessManagerDependenciesFactory
                                                       .StateDependencyFactory
                                                       .SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            return this;
        }
    }
}