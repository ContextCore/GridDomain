using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.EventsUpgrade;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.EventsUpgrade
{
    public class Cluster_Future_events_upgraded_by_object_adapter : Future_events_upgraded_by_object_adapter
    {
        public Cluster_Future_events_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(ConfigureDomain(new BalanceFixture(output, new PersistedQuartzConfig())).Clustered())
        { }
    }
}