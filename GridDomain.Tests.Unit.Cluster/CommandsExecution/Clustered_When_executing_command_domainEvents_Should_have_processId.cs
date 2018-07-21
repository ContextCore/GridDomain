using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Clustered_When_executing_command_domainEvents_Should_have_processId : When_executing_command_domainEvents_Should_have_processId
    {
        public Clustered_When_executing_command_domainEvents_Should_have_processId(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered().LogToFile(nameof(Clustered_When_executing_command_domainEvents_Should_have_processId))) {}
    }
}