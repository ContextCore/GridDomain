using System;
using System.Threading.Tasks;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Acceptance.Tools;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.CommandsExecution;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance
{
    
    public class CommandIndepotenceTests : NodeTestKit
    {
        //disable messages serialization dut to wire exception on deserealizeing SqlException 
        //when same command is executed second time and we trying to write it status to DB
        public CommandIndepotenceTests(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new AcceptanceAutoTestNodeConfiguration(),cfg =>
                                                                                   ActorSystemBuilder.New()
                                                                                   .Log(LogEventLevel.Debug)
                                                                                   .DomainSerialization()
                                                                                   .RemoteActorProvider()
                                                                                   .Remote(new NodeNetworkAddress())
                                                                                   .SqlPersistence(new AutoTestNodeDbConfiguration())
                                                                                   .BuildHocon(),
                                       new []{new BalloonDomainConfiguration()})
                   .UseSqlPersistence()) {}
        
        [Fact]
        public async Task Given_aggregate_When_executing_same_command_twice_Then_second_commands_fails()
        {
            var cmd = new WriteTitleCommand(43, Guid.NewGuid().ToString());

            await Node.Execute(cmd);
            await Task.Delay(1000); //allow command state actor to terminate
            await Node.Execute(cmd).ShouldThrow<CommandAlreadyExecutedException>();
          
        } 
        [Fact]
        public async Task Given_aggregate_When_executing_same_command_several_times_fast_Then_only_first_command_succeed()
        {
            var cmd = new WriteTitleCommand(43, Guid.NewGuid().ToString());

            await Node.Execute(cmd);
            await Node.Execute(cmd).ShouldThrow<CommandAlreadyExecutedException>();
            await Node.Execute(cmd).ShouldThrow<CommandAlreadyExecutedException>();
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