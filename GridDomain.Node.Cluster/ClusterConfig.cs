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
        
        public AkkaCluster CreateCluster(Action<ActorSystem> additionalInit = null)
        {
            if(_autoSeedNodes.Count + _seedNodes.Count == 0)
                throw new InvalidOperationException("Specify at least one seed node");
            
            var seedNodeActorSystems = SeedNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            var workerNodesActorSystems = WorkerNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            var autoSeedActorSystems = AutoSeedNodes.Select(b => b.BuildClusterSystemFactory(Name).Create()).ToArray();
            
            if(additionalInit != null)
                foreach (var s in seedNodeActorSystems.Concat(workerNodesActorSystems)
                                                      .Concat(autoSeedActorSystems))
                    additionalInit(s);
            
            var akkaCluster = new AkkaCluster(seedNodeActorSystems.FirstOrDefault() ?? autoSeedActorSystems.First());
            
            foreach(var sys in autoSeedActorSystems) 
                akkaCluster.JoinSeed(sys);
            
            foreach(var sys in workerNodesActorSystems) 
                akkaCluster.JoinWorker(sys);
            
            return akkaCluster;
        }
    }
}