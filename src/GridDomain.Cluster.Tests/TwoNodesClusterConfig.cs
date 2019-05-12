using Akka.Cluster.TestKit;
using Akka.Remote.TestKit;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using Serilog;

namespace GridDomain.Cluster.Tests
{
    public class TwoNodesClusterConfig : MultiNodeConfig
    {
        public readonly RoleName Seed;
        public readonly RoleName Worker;

        public TwoNodesClusterConfig()
        {
            Seed = Role("seed");
            Worker = Role("worker");

            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            var seed = new ActorSystemConfigBuilder()
                .Add(LogConfig.All())
                .Add(new ClusterActorProviderConfig())
                .Build();

            seed = seed.WithFallback(MultiNodeClusterSpec.ClusterConfig());
               
            var worker = new ActorSystemConfigBuilder()
                .Add(LogConfig.All())
                .Add(new ClusterActorProviderConfig())
                .Build();
                
            worker = worker.WithFallback(MultiNodeClusterSpec.ClusterConfig());
                
            NodeConfig(new[] { Seed }, new []{seed});
            NodeConfig(new[] { Worker }, new []{worker});
        }
    }
    
    
    
}