using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_execute_command_Then_aggregate_Should_persist_changed : When_execute_command_Then_aggregate_Should_persist_changed
    {
        public Cluster_When_execute_command_Then_aggregate_Should_persist_changed(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}