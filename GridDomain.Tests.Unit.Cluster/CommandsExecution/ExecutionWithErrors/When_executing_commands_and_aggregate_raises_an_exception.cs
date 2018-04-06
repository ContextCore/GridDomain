using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    //different fixtures from static method ? 
    public class Cluster_When_executing_commands_and_aggregate_raises_an_exception : When_executing_commands_and_aggregate_raises_an_exception
    {
        public Cluster_When_executing_commands_and_aggregate_raises_an_exception(ITestOutputHelper helper)
            : base(new BalloonFixture(helper).Clustered()) { }
    }
}