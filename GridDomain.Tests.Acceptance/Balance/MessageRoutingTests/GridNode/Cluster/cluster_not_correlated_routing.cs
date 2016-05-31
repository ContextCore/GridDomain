using System.Linq;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.Cluster.Setup;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.Cluster
{
    class Cluster_not_correlated_routing : ClusterActorSystemTest
    {
        protected override IRouterConfiguration CreateRoutes()
        {
            return new NotCorrelatedRouting<ClusterMessage, TestHandler>();
        }
    }
}