using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout :
        When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout
    {
        public Cluster_When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout(ITestOutputHelper output)
            :base(new NodeTestFixture(output).Clustered()){}
     
    }
}