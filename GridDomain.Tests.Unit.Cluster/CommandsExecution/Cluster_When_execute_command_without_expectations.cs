using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_execute_command_without_expectations : When_execute_command_without_expectations
    {
        public Cluster_When_execute_command_without_expectations(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
      
    }
}