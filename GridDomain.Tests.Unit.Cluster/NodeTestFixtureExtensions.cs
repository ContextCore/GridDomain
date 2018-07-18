using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.Transport;

namespace GridDomain.Tests.Unit.Cluster
{
    public static class NodeTestFixtureExtensions
    {
        public static T Clustered<T>(this T fxt)where T:NodeTestFixture
        {
            fxt.ActorSystemConfigBuilder = fxt.ActorSystemConfigBuilder.ConfigureCluster(fxt.NodeConfig.Name);
            fxt.NodeBuilder = new ClusterNodeBuilder((GridNodeBuilder)fxt.NodeBuilder);
            fxt.NodeBuilder.Transport(sys => sys.InitDistributedTransport());
            fxt.TestNodeBuilder = (n, kit) => new TestClusterNode((GridClusterNode) n, kit);

            return fxt;
        }
    }
}