using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluster_Aggregate_Should_Not_save_snapshots_on_message_process_by_default : Aggregate_Should_Not_save_snapshots_on_message_process_by_default
    {
        public Cluster_Aggregate_Should_Not_save_snapshots_on_message_process_by_default(ITestOutputHelper output)
            : base(new BalloonFixture(output).Clustered()) {}

    }
}