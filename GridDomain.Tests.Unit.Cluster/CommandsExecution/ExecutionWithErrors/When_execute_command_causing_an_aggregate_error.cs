using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    public class Cluster_When_execute_command_causing_an_aggregate_error : When_execute_command_causing_an_aggregate_error
    {
        public Cluster_When_execute_command_causing_an_aggregate_error(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}