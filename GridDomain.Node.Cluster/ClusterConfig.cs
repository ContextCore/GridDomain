using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;

namespace GridDomain.Node.Cluster {
    public class ClusterConfig
    {
        private readonly List<ActorSystemBuilder> _seedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _autoSeedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _workerNodes = new List<ActorSystemBuilder>();

        public ClusterConfig(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
        public IReadOnlyCollection<ActorSystemBuilder> SeedNodes => _seedNodes;
        public IReadOnlyCollection<ActorSystemBuilder> AutoSeedNodes => _autoSeedNodes;
        public IReadOnlyCollection<ActorSystemBuilder> WorkerNodes => _workerNodes;
        
        public string[] CreateConfigs()
        {
            return SeedNodes.Concat(WorkerNodes)
                            .Select(n => n.BuildHocon())
                            .ToArray();
        }

        public void AddAutoSeed(params ActorSystemBuilder[] builder)
        {
            _autoSeedNodes.AddRange(builder);
        }
        public void AddSeed(params ActorSystemBuilder[] builder)
        {
            _seedNodes.AddRange(builder);
        }
        public void AddWorker(params ActorSystemBuilder[] builder)
        {
            _workerNodes.AddRange(builder);
        }
        
        public ClusterInfo CreateCluster(Action<ActorSystem> additionalInit = null)
        {
            if(_autoSeedNodes.Count + _seedNodes.Count == 0)
                throw new InvalidOperationException("Specify at least one seed node");
            
            var seedNodeActorSystems = SeedNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            var workerNodesActorSystems = WorkerNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            var autoSeedActorSystems = AutoSeedNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            var actorSystems = seedNodeActorSystems.Concat(workerNodesActorSystems)
                                                   .Concat(autoSeedActorSystems).ToArray();
            if(additionalInit != null)
            {
                foreach (var s in actorSystems)
                    additionalInit(s);
            }

            var akkaCluster = Akka.Cluster.Cluster.Get(seedNodeActorSystems.FirstOrDefault() ?? autoSeedActorSystems.First());
            
             akkaCluster.JoinSeedNodesAsync(autoSeedActorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress)).Wait();
            
             foreach(var workerAddress in workerNodesActorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress))
             akkaCluster.JoinAsync(workerAddress).Wait();
            
            return new ClusterInfo(akkaCluster,actorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress).ToArray());
        }

     
    }
    public class ClusterInfo
    {
        public ClusterInfo(Akka.Cluster.Cluster cluster, IReadOnlyCollection<Address> members)
        {
            Cluster = cluster;
            Members = members;
        }
        public Akka.Cluster.Cluster Cluster { get; }
        public IReadOnlyCollection<Address> Members { get; }
    }
}