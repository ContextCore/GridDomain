using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_AsyncExecute_without_timeout_using_node_defaults : AsyncExecute_without_timeout_using_node_defaults
    {
        public Cluster_AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : base(
            new NodeTestFixture(output,null, TimeSpan.FromMilliseconds(1)).Clustered()) {}

    }
}