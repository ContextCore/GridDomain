using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.Cluster
{
    public class AkkaCluster : IDisposable
    {
        public ActorSystem[] NonSeedNodes { get; }
        public ActorSystem[] SeedNodes { get; }
        private readonly ActorSystem[] _allNodes;
        public AkkaCluster(ActorSystem[] seedNodes, ActorSystem[] nonSeedNodes)
        {
            SeedNodes = seedNodes;
            NonSeedNodes = nonSeedNodes;
            _allNodes = seedNodes.Concat(nonSeedNodes).ToArray();
            All = _allNodes;
        }

        public IReadOnlyCollection<ActorSystem> All { get; } 

        public void Dispose()
        {
            CoordinatedShutdown.Get(RandomNode()).Run().Wait();
        }

        public Akka.Cluster.Cluster RandomNodeCluster()
        {
            return Akka.Cluster.Cluster.Get(RandomNode());
        }
        
        public ActorSystem RandomNode()
        {
            var index = new Random().Next(_allNodes.Length-1);
            return _allNodes[index];
        }
    }
}