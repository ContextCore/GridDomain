using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Acceptance.Tools;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluster_Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy : Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy
    {
        public Cluster_Aggregate_Should_save_snapshots_after_each_message_according_to_save_policy(ITestOutputHelper output)
            : base(new BalloonFixture(output).UseSqlPersistence()
                                             .EnableSnapshots()
                                             .Clustered()) { }
    }
}