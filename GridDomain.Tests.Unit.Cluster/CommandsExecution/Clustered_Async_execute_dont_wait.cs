using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Clustered_Async_execute_dont_wait : Async_execute_dont_wait
    {
        public Clustered_Async_execute_dont_wait(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
    }
}