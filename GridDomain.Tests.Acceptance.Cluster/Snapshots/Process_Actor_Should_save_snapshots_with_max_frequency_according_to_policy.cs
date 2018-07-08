using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Snapshots
{
    public class Cluter_Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy : Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy
    {
        public Cluter_Process_Actor_Should_save_snapshots_with_max_frequency_according_to_policy(ITestOutputHelper output)
            : base(ConfigureFixture(new SoftwareProgrammingProcessManagerFixture(output))
                       .Clustered()) { }
    }
}