using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;

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
        
        public async Task<ClusterInfo> CreateCluster(Action<ActorSystem> additionalInit = null)
        {
            if(_autoSeedNodes.Count + _seedNodes.Count == 0)
                throw new InvalidOperationException("Specify at least one seed node");

            var actorSystemBuilders = SeedNodes.Concat(WorkerNodes).Concat(AutoSeedNodes).ToArray();
            foreach(var builder in actorSystemBuilders)
                builder.Add(new MinMembersInCluster(actorSystemBuilders.Length));
                    
                    
            var seedNodeActorSystems = SeedNodes.Select(b => b.BuildClusterSystemFactory(Name).CreateSystem()).ToArray();
            var workerNodesActorSystems = WorkerNodes.Select(b => b.BuildClusterSystemFactory(Name).CreateSystem()).ToArray();
            var autoSeedActorSystems = AutoSeedNodes.Select(b => b.BuildClusterSystemFactory(Name).CreateSystem()).ToArray();
            var actorSystems = seedNodeActorSystems.Concat(workerNodesActorSystems)
                                                   .Concat(autoSeedActorSystems).ToArray();
            if(additionalInit != null)
            {
                foreach (var s in actorSystems)
                    additionalInit(s);
            }

            bool clusterReady = false;
            var akkaCluster = Akka.Cluster.Cluster.Get(seedNodeActorSystems.FirstOrDefault() ?? autoSeedActorSystems.First());
              akkaCluster.RegisterOnMemberUp(() =>clusterReady = true);
            
           
            foreach(var workerAddress in workerNodesActorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress))
                 await akkaCluster.JoinAsync(workerAddress);

            if(autoSeedActorSystems.Any())
                await akkaCluster.JoinSeedNodesAsync(autoSeedActorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress));

            while (!clusterReady)
                await Task.Delay(500);
            
            return new ClusterInfo(akkaCluster,actorSystems.Select(s => ((ExtendedActorSystem)s).Provider.DefaultAddress).ToArray());
        }
    }

    public class MinMembersInCluster : IHoconConfig
    {
        private int _length;

        public MinMembersInCluster(int length)
        {
            _length = length;
        }

        public string Build()
        {
            return $"cluster.min-nr-of-members = {_length}";
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