using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_CommandWaiter_picks_events_by_correlation_id_by_default : CommandWaiter_picks_events_by_correlation_id_by_default
    {
        public Cluster_CommandWaiter_picks_events_by_correlation_id_by_default(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()) {}

    }
}