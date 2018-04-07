using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{

    public class Cluster_Async_execution_dont_block_aggregate : Async_execution_dont_block_aggregate
    {
        public Cluster_Async_execution_dont_block_aggregate(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
    }
}