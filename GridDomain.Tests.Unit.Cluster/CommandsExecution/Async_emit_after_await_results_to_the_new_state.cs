using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution {
    public class Cluster_Async_emit_after_await_results_to_the_new_state : Async_emit_after_await_results_to_the_new_state
    {
        public Cluster_Async_emit_after_await_results_to_the_new_state(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) { }
    }
}