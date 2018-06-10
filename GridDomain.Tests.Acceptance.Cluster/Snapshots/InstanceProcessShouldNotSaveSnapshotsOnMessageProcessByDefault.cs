using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluster_InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault : InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault
    {
        public Cluster_InstanceProcessShouldNotSaveSnapshotsOnMessageProcessByDefault(ITestOutputHelper output)
            : base(new SoftwareProgrammingProcessManagerFixture(output).Clustered()) { }
    }
}