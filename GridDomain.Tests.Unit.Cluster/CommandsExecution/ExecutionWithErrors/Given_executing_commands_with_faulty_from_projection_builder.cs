using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    public class Cluster_Given_executing_commands_with_faulty_from_projection_builder : Given_executing_commands_with_faulty_from_projection_builder
    {
        public Cluster_Given_executing_commands_with_faulty_from_projection_builder(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new FaultyBalloonProjectionDomainConfiguration()).Clustered()) {}
    }
}