using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.EventsUpgrade;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    public class Cluster_Future_events_upgraded_by_object_adapter : Future_events_upgraded_by_object_adapter
    {
        public Cluster_Future_events_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(ConfigureDomain(new BalanceFixture(output, new PersistedQuartzConfig())).Clustered())
        { }
    }
}