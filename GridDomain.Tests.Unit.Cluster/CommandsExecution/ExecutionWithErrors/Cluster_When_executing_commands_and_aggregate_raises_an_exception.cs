using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution.ExecutionWithErrors
{
    //different fixtures from static method ? 
    public class Cluster_When_executing_commands_and_aggregate_raises_an_exception : When_executing_commands_and_aggregate_raises_an_exception
    {
        public Cluster_When_executing_commands_and_aggregate_raises_an_exception(ITestOutputHelper helper)
            : base(new BalloonFixture(helper).Clustered().LogToFile(nameof(Cluster_When_executing_commands_and_aggregate_raises_an_exception))) { }
    }
}