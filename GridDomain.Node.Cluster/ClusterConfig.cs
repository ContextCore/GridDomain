using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Configuration;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node.Cluster
{
    public class ClusterConfig
    {
        private readonly List<ActorSystemBuilder> _seedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _autoSeedNodes = new List<ActorSystemBuilder>();
        private readonly List<ActorSystemBuilder> _workerNodes = new List<ActorSystemBuilder>();
        public readonly ILogger Logger;
        private Func<ActorSystem, Task> _onMemberUp = s => Task.CompletedTask;
        private Func<ActorSystem, Task> _additionalInit = s => Task.CompletedTask;

        public ClusterConfig(string name, ILogger log)
        {
            Logger = log;
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

        public ClusterConfig OnClusterUp(Func<ActorSystem, Task> sys)
        {
            _onMemberUp += sys;
            return this;
        }

        public ClusterConfig AdditionalInit(Func<ActorSystem, Task> sys)
        {
            _additionalInit += sys;
            return this;
        }

        public async Task<ClusterInfo> Create()
        {
            var actorSystemBuilders = SeedNodes.Concat(WorkerNodes)
                                               .Concat(AutoSeedNodes)
                                               .ToArray();

            foreach (var cfg in SeedNodes.Concat(AutoSeedNodes))
            {
                cfg.Add(new MinMembersInCluster(actorSystemBuilders.Length));
            }


            Func<ActorSystem, Task<ActorSystem>> init = (async s =>
                                                         {
                                                             await _additionalInit(s);
                                                             return s;
                                                         });


            var seedSystems = await CreateSystems(SeedNodes, init);
            var seedSystemAddresses = seedSystems.Select(s => s.GetAddress())
                                                 .ToArray();

            var autoSeedSystems = await CreateSystems(AutoSeedNodes, init);
            var autoSeedAddresses = autoSeedSystems.Select(s => s.GetAddress())
                                                   .ToArray();

            var workerSystems = await CreateSystems(WorkerNodes, init);
            var workerSystemAddresses = workerSystems.Select(s => s.GetAddress())
                                                     .ToArray();

            var allActorSystems = seedSystems.Concat(autoSeedSystems)
                                             .Concat(workerSystems);

            var leader = seedSystems.FirstOrDefault() ?? throw new CannotDetermineLeaderException();

            bool clusterReady = false;

            var akkaCluster = Akka.Cluster.Cluster.Get(leader);
            akkaCluster.RegisterOnMemberUp(async () =>
                                           {
                                               foreach (var systemBuilder in allActorSystems)
                                               {
                                                   await _onMemberUp(systemBuilder);
                                               }

                                               clusterReady = true;
                                           });

            foreach (var address in autoSeedAddresses)
                await akkaCluster.JoinSeedNodesAsync(new[] {address});

            foreach (var address in workerSystemAddresses)
                await akkaCluster.JoinAsync(address);

            foreach (var systemn in allActorSystems)
            {
                Logger.Information(systemn.Settings.ToString());
            }

            while (!clusterReady)
                await Task.Delay(500);

            return new ClusterInfo(akkaCluster,
                                   seedSystemAddresses.Concat(autoSeedAddresses)
                                                      .Concat(workerSystemAddresses)
                                                      .ToArray());
        }

        private async Task<ActorSystem[]> CreateSystems(IReadOnlyCollection<ActorSystemBuilder> actorSystemBuilders, Func<ActorSystem, Task<ActorSystem>> init)
        {
            var systems = actorSystemBuilders.Select(s => s.BuildClusterSystemFactory(Name)
                                                           .CreateSystem())
                                             .ToArray();
            foreach (var s in systems)
                await init(s);

            return systems;
        }
    }
}