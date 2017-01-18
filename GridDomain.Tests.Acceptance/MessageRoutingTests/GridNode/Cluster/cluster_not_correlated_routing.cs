using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster
{
    [Ignore("Cluster is not supported for now")]
    internal class Cluster_not_correlated_routing : ClusterActorSystemTest
    {
        protected override IRouterConfiguration CreateRoutes()
        {
            return new NotCorrelatedRouting<ClusterEvent, TestHandler>();
        }
    }
}