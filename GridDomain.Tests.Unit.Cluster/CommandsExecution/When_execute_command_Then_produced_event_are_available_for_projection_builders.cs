using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_execute_command_Then_produced_event_are_available_for_projection_builders : When_execute_command_Then_produced_event_are_available_for_projection_builders
    {
        public Cluster_When_execute_command_Then_produced_event_are_available_for_projection_builders(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) { }
    }
}