using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tools;
using GridDomain.Tools.Repositories.AggregateRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Snapshots
{
    public class Cluster_Instance_process_Should_recover_from_snapshot : Instance_process_Should_recover_from_snapshot
    {
        public Cluster_Instance_process_Should_recover_from_snapshot(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).UseSqlPersistence().Clustered()) {}
    }
}