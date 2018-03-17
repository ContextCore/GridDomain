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
        private readonly IList<ActorSystem> _allNodes = new List<ActorSystem>();
        public AkkaCluster(ActorSystem[] seedNodes, ActorSystem[] nonSeedNodes)
        {
            SeedNodes = seedNodes;
            NonSeedNodes = nonSeedNodes;
            _allNodes = seedNodes.Concat(nonSeedNodes)
                                 .ToList();
        }

        public ActorSystem[] All => SeedNodes.Concat(NonSeedNodes).ToArray();

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
            var index = new Random().Next(_allNodes.Count-1);
            return _allNodes[index];
        }
    }
}