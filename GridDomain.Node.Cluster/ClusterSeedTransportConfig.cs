using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Cluster
{
    
    public class ClusterConfig
    {
        public ClusterConfig(string name, IReadOnlyCollection<ActorSystemBuilder> seedNodes, IReadOnlyCollection<ActorSystemBuilder> workerNodes)
        {
            Name = name;
            SeedNodes = seedNodes;
            WorkerNodes = workerNodes;
        }
        public string Name { get; }
        public IReadOnlyCollection<ActorSystemBuilder> SeedNodes { get; }
        public IReadOnlyCollection<ActorSystemBuilder> WorkerNodes { get; }

        public string[] CreateConfigs()
        {
            return SeedNodes.Concat(WorkerNodes)
                            .Select(n => n.BuildHocon())
                            .ToArray();
        }
        
        public AkkaCluster CreateCluster(Action<ActorSystem> additionalInit = null)
        {
            var seedNodeActorSystems = SeedNodes.Select(b => b.BuildActorSystemFactory(Name).Create()).ToArray();
            var workerNodesActorSystems = WorkerNodes.Select(b => b.BuildActorSystemFactory(Name).Create()).ToArray();
            
            if(additionalInit != null)
                foreach (var s in seedNodeActorSystems.Concat(workerNodesActorSystems))
                    additionalInit(s);

            return new AkkaCluster(seedNodeActorSystems,
                                   workerNodesActorSystems);
        }
    }
    
    
    public class AutoTerminateProcessOnClusterShutdown : IHoconConfig
    {
        public string Build()
        {
            return "akka.coordinated-shutdown.exit-clr = on";
        }
    }

    public class ClusterSeedTransportConfig : IHoconConfig
    {
        private readonly string[] _seedNodeFullAddresses;

        public ClusterSeedTransportConfig(params string[] seedNodeFullAddresses)
        {
            _seedNodeFullAddresses = seedNodeFullAddresses;
        }

        public string Build()
        {
            var seeds = string.Join(Environment.NewLine, _seedNodeFullAddresses.Select(n => @"""" + n + @""""));

            var clusterConfigString = @"actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
            cluster {
                            seed-nodes = [" + seeds + @"]
            }";

            return clusterConfigString;
        }
     
    }
}