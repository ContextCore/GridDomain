using System;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Clustered_Async_execute_dont_wait : Async_execute_dont_wait
    {
        public Clustered_Async_execute_dont_wait(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
    }
}