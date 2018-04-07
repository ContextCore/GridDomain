using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Clustered_When_executing_command_domainEvents_Should_have_processId : When_executing_command_domainEvents_Should_have_processId
    {
        public Clustered_When_executing_command_domainEvents_Should_have_processId(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}