using GridDomain.Tests.Unit.CommandsExecution;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_Async_emit_after_await_results_to_the_new_state : Async_emit_after_await_results_to_the_new_state
    {
        public Cluster_Async_emit_after_await_results_to_the_new_state(ITestOutputHelper output) : base(
                                                       new NodeTestFixture(output).Clustered()) { }
    }
}