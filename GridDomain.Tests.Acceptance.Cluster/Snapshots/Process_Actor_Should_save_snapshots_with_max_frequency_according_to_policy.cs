using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Acceptance.EventsUpgrade;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.SnapshotRepositories;
using Xunit;
using Xunit.Abstractions;
using GridDomain.Transport.Remote;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluter_Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy : Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy
    {
        public Cluter_Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(ConfigureFixture(new SoftwareProgrammingProcessManagerFixture(output))
                       .Clustered()) { }
    }
}