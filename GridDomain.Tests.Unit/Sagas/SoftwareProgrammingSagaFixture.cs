using System;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration;

namespace GridDomain.Tests.Unit.Sagas
{
    public class SoftwareProgrammingSagaFixture : NodeTestFixture
    {
        private readonly SoftwareProgrammingSagaDomainConfiguration _softwareProgrammingSagaDomainConfiguration;

        public SoftwareProgrammingSagaFixture(IDomainConfiguration config = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, timeout)
        {
            _softwareProgrammingSagaDomainConfiguration = new SoftwareProgrammingSagaDomainConfiguration(Logger);
            Add(new BalloonDomainConfiguration());
            Add(new SchedulingConfiguration(new InMemoryQuartzConfig()));
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(_softwareProgrammingSagaDomainConfiguration);
            return base.CreateNodeSettings();
        }

        public SoftwareProgrammingSagaFixture InitSnapshots(int keep = 1,
                                                            TimeSpan? maxSaveFrequency = null,
                                                            int saveOnEach = 1)
        {
            _softwareProgrammingSagaDomainConfiguration.SoftwareProgrammingSagaDependenciesFactory
                                                       .StateDependencyFactory
                                                       .SnapshotPolicyCreator = () => new SnapshotsPersistencePolicy(saveOnEach, keep, maxSaveFrequency);
            return this;
        }
    }
}