using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster.Setup;
using GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.SingleSystem.Setup;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests.GridNode.Cluster
{
    internal class Cluster_not_correlated_routing : ClusterActorSystemTest
    {
        protected override IRouterConfiguration CreateRoutes()
        {
            return new NotCorrelatedRouting<ClusterMessage, TestHandler>();
        }
    }
}