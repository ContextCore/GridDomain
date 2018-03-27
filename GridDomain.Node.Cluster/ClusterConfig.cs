using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Hocon;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node.Cluster
{

    public static class ActorSystemExtensions
    {
        public static Address GetAddress(this ActorSystem sys)
        {
            return ((ExtendedActorSystem) sys).Provider.DefaultAddress;
        }
    }
    public class ClusterConfig
    {
        private readonly List<ActorSystemBuilder> _seedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _autoSeedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _workerNodes = new List<ActorSystemBuilder>();
        private ILogger _logger;
        public ClusterConfig(string name, ILogger log)
        {
            _logger = log;
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
            var actorSystemBuilders = SeedNodes.Concat(WorkerNodes)
                                               .Concat(AutoSeedNodes)
                                               .ToArray();

            foreach (var cfg in actorSystemBuilders)
            {
                cfg.Add(new MinMembersInCluster(actorSystemBuilders.Length));
            }


            Func<ActorSystem, ActorSystem> init = additionalInit == null
                                                      ? (Func<ActorSystem, ActorSystem>) (s => s)
                                                      : (s =>
                                                         {
                                                             additionalInit(s);
                                                             return s;
                                                         });


            var seedSystems = CreateSystems(SeedNodes, init);
            var seedSystemAddresses = seedSystems.Select(s => s.GetAddress())
                                                 .ToArray();

            var autoSeedSystems = CreateSystems(AutoSeedNodes, init);
            var autoSeedAddresses = autoSeedSystems.Select(s => s.GetAddress()).ToArray();

            var workerSystems = CreateSystems(WorkerNodes, init);
            var workerSystemAddresses = workerSystems.Select(s => s.GetAddress()).ToArray();

            var leader = seedSystems.FirstOrDefault() ?? throw new CannotDetermineLeaderException();

            bool clusterReady = false;
          
            var akkaCluster = Akka.Cluster.Cluster.Get(leader);
            akkaCluster.RegisterOnMemberUp(() => clusterReady = true);

            foreach (var address in autoSeedAddresses)
                await akkaCluster.JoinSeedNodesAsync(new[] {address});

            foreach (var address in workerSystemAddresses)
                await akkaCluster.JoinAsync(address);

            foreach (var cfg in seedSystems.Concat(autoSeedSystems).Concat(workerSystems))
            {
                _logger.Information(cfg.Settings.ToString());
            }
            
            while (!clusterReady)
                await Task.Delay(500);

            return new ClusterInfo(akkaCluster,
                                   seedSystemAddresses.Concat(autoSeedAddresses)
                                                      .Concat(workerSystemAddresses)
                                                      .ToArray());
        }

        private ActorSystem[] CreateSystems(IReadOnlyCollection<ActorSystemBuilder> actorSystemBuilders, Func<ActorSystem, ActorSystem> init)
        {
            return actorSystemBuilders.Select(s => s.BuildClusterSystemFactory(Name)
                                                    .CreateSystem())
                                      .Select(init)
                                      .ToArray();
        }
    }

    public class CannotDetermineLeaderException : Exception { }

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