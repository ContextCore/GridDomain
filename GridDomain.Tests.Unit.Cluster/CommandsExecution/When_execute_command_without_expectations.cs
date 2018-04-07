using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using GridDomain.Tests.Unit.EventsUpgrade;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_execute_command_without_expectations : When_execute_command_without_expectations
    {
        public Cluster_When_execute_command_without_expectations(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
      
    }
}