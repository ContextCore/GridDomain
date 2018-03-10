using System;
using System.Threading.Tasks;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.CommandsExecution;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance
{
    
    public class CommandIndepotenceTests : NodeTestKit
    {
        public CommandIndepotenceTests(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new BalloonDomainConfiguration()).UseSqlPersistence()) {}
        
        
        [Fact]
        public async Task Given_aggregate_When_executing_same_command_twice_Then_second_commands_fails()
        {
            var cmd = new WriteTitleCommand(43, Guid.NewGuid().ToString());

            await Node.Execute(cmd);
            await Task.Delay(1000); //allow command state actor to terminate
            await Node.Execute(cmd).ShouldThrow<CommandAlreadyExecutedException>();
        }
        
    }
    
    public class AutoTestLocalDbConfiguration : IDbConfiguration
    {
        private const string JournalConnectionStringName = "ReadModel";
        
        public string ReadModelConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ??  @"Data Source=localhost,1400;Initial Catalog=AutoTestRead;User = sa; Password = P@ssw0rd1;";

        public string LogsConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? @"Data Source=localhost,1400;Initial Catalog=AutoTestLogs;User = sa; Password = P@ssw0rd1;";
    }
}