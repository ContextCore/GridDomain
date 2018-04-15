using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller:When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller
    {
        public Cluster_When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}