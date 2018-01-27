using System;
using System.Threading.Tasks;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution {
    public class CommandIndepotenceTests : BalloonDomainCommandExecutionTests
    {
        public CommandIndepotenceTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Given_aggregate_When_executing_same_command_twice_Then_second_commands_fails()
        {
            var cmd = new WriteTitleCommand(43, Guid.NewGuid());

            await Node.Execute(cmd);
            await Node.Execute(cmd).ShouldThrow<CommandAlreadyExecutedException>();
        }
        
    }
}