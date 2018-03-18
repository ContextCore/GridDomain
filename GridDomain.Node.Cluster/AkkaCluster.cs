using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace GridDomain.Node.Cluster
{
    
    
    // define envelope used to message routing

    public class AkkaCluster : IDisposable
    {
        public IReadOnlyCollection<ActorSystem> NonSeedNodes => _nonSeedNodes;
        public IReadOnlyCollection<ActorSystem> SeedNodes => _seedNodes;
        public IReadOnlyCollection<ActorSystem> All => _allNodes; 

        private readonly List<ActorSystem> _allNodes = new List<ActorSystem>();
        private readonly List<ActorSystem> _nonSeedNodes = new List<ActorSystem>();
        private readonly List<ActorSystem> _seedNodes = new List<ActorSystem>();
        private Akka.Cluster.Cluster _cluster;

        public AkkaCluster(ActorSystem system, params ActorSystem[] preconfigiredSeedNodes)
        {
            _cluster = Akka.Cluster.Cluster.Get(system);
            _seedNodes.Add(system);
            if(preconfigiredSeedNodes != null)
                _seedNodes.AddRange(preconfigiredSeedNodes);
        }

        public void JoinSeed(ActorSystem sys)
        {
            _seedNodes.Add(sys);
            _allNodes.Add(sys);
            var address = ((ExtendedActorSystem) sys).Provider.DefaultAddress;
            _cluster.JoinSeedNodes(new []{address});
        }
        
        public void JoinWorker(ActorSystem sys)
        {
            _nonSeedNodes.Add(sys);
            _allNodes.Add(sys);
            var address = ((ExtendedActorSystem) sys).Provider.DefaultAddress;
            _cluster.Join(address);
        }
        
        public void Dispose()
        {
           // CoordinatedShutdown.Get(RandomNode()).Run().Wait();
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