using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_CommandWaiter_picks_events_by_correlation_id_by_default : CommandWaiter_picks_events_by_correlation_id_by_default
    {
        public Cluster_CommandWaiter_picks_events_by_correlation_id_by_default(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()) {}

    }
}