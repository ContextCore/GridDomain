using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tools;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluster_Aggregate_should_recover_from_snapshot : Aggregate_should_recover_from_snapshot
    {
        public Cluster_Aggregate_should_recover_from_snapshot(ITestOutputHelper output)
            : base(ConfigureFixture(new BalloonFixture(output))
                       .Clustered()) { }
    }
}