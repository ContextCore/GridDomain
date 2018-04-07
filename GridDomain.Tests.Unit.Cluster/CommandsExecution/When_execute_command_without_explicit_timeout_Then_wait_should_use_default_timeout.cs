using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout :
        When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout
    {
        public Cluster_When_execute_command_without_explicit_timeout_Then_wait_should_use_default_timeout(ITestOutputHelper output)
            :base(new NodeTestFixture(output).Clustered()){}
     
    }
}