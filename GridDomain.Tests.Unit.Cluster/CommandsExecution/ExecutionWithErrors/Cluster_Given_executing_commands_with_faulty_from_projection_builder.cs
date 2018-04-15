using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution.ExecutionWithErrors
{
    public class Cluster_Given_executing_commands_with_faulty_from_projection_builder : Given_executing_commands_with_faulty_from_projection_builder
    {
        public Cluster_Given_executing_commands_with_faulty_from_projection_builder(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new FaultyBalloonProjectionDomainConfiguration()).Clustered()) {}
    }
}