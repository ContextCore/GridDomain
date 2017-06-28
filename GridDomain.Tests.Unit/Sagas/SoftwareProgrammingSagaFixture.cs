using System;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.Sagas
{
    public class SoftwareProgrammingSagaFixture : NodeTestFixture
    {
        public SoftwareProgrammingSagaFixture(IDomainConfiguration config = null,
                                              TimeSpan? timeout = default(TimeSpan?)) : base(config, timeout)
        {
            Add(new SoftwareProgrammingSagaDomainConfiguration(Logger));
            Add(new BalloonDomainConfiguration());
            Add(new QuartzSchedulerConfiguration(new InMemoryQuartzConfig()));
        }
    }
}