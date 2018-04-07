using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller:When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller
    {
        public Cluster_When_execute_command_expecting_several_events_Then_all_of_them_should_be_availabe_for_caller(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}