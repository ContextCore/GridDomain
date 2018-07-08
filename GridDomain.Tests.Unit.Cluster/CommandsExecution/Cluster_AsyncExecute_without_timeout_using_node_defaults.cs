using System;
using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_AsyncExecute_without_timeout_using_node_defaults : AsyncExecute_without_timeout_using_node_defaults
    {
        public Cluster_AsyncExecute_without_timeout_using_node_defaults(ITestOutputHelper output) : base(
            new NodeTestFixture(output,null, TimeSpan.FromMilliseconds(1)).Clustered()) {}

    }
}