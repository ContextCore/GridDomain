using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node.Cluster.Configuration.Hocon;
using GridDomain.Node.Configuration;
using GridDomain.Node.Logging;
using Serilog;

namespace GridDomain.Node.Cluster.Configuration
{
    /// <summary>
    /// Create a cluster from several actor systems hosted in one machine (local)
    /// </summary>
    public class ClusterConfig
    {
        private readonly List<IActorSystemConfigBuilder> _seedNodes = new List<IActorSystemConfigBuilder>();
        private readonly List<IActorSystemConfigBuilder> _autoSeedNodes = new List<IActorSystemConfigBuilder>();
        private readonly List<IActorSystemConfigBuilder> _workerNodes = new List<IActorSystemConfigBuilder>();

        private readonly List<Func<ActorSystem, Task>>  _onMemberUp = new List<Func<ActorSystem, Task>>{s => Task.CompletedTask};
        private readonly List<Func<ActorSystem, Task>> _additionalInit =new List<Func<ActorSystem, Task>>{s => Task.CompletedTask};

        public ClusterConfig(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public IReadOnlyCollection<IActorSystemConfigBuilder> SeedNodes => _seedNodes;
        public IReadOnlyCollection<IActorSystemConfigBuilder> AutoSeedNodes => _autoSeedNodes;
        public IReadOnlyCollection<IActorSystemConfigBuilder> WorkerNodes => _workerNodes;

        public IEnumerable<IActorSystemConfigBuilder> AllNodes => SeedNodes.Union(AutoSeedNodes)
                                                                           .Union(WorkerNodes);

        public void AddAutoSeed(params IActorSystemConfigBuilder[] configBuilder)
        {
            _autoSeedNodes.AddRange(configBuilder);
        }

        public void AddSeed(params IActorSystemConfigBuilder[] configBuilder)
        {
            _seedNodes.AddRange(configBuilder);
        }

        public void AddWorker(params IActorSystemConfigBuilder[] configBuilder)
        {
            _workerNodes.AddRange(configBuilder);
        }

        public ClusterConfig OnClusterUp(Func<ActorSystem, Task> sys)
        {
            _onMemberUp.Add(sys);
            return this;
        }

        public ClusterConfig Log(Func<ActorSystem,ILogger> logProduser)
        {
            AdditionalInit(s =>
                           {
                               s.InitSerilogExtension(logProduser(s));
                               return Task.CompletedTask;
                           });
            return this;
        }

        public ClusterConfig AdditionalInit(Func<ActorSystem, Task> sys)
        {
            _additionalInit.Add(sys);
            return this;
        }

        private async Task PerformAdditionInitialization(ActorSystem sys)
        {
            foreach (var init in _additionalInit)
                await init(sys);
        }
        
        private async Task PerformOnMemberUp(ActorSystem sys)
        {
            foreach (var init in _onMemberUp)
                await init(sys);
        }
        
        public async Task<ClusterInfo> CreateCluster()
        {
            var actorSystemBuilders = SeedNodes.Concat(WorkerNodes)
                                               .Concat(AutoSeedNodes)
                                               .ToArray();

            foreach (var cfg in SeedNodes.Concat(AutoSeedNodes))
            {
                cfg.Add(new MinMembersInCluster(actorSystemBuilders.Length));
            }


            var seedSystems = await CreateSystems(SeedNodes, PerformAdditionInitialization);
            var seedSystemAddresses = seedSystems.Select(s => s.GetAddress())
                                                 .ToArray();

            var autoSeedSystems = await CreateSystems(AutoSeedNodes, PerformAdditionInitialization);
            var autoSeedAddresses = autoSeedSystems.Select(s => s.GetAddress())
                                                   .ToArray();

            var workerSystems = await CreateSystems(WorkerNodes, PerformAdditionInitialization);
            var workerSystemAddresses = workerSystems.Select(s => s.GetAddress())
                                                     .ToArray();

            var allActorSystems = seedSystems.Concat(autoSeedSystems)
                                             .Concat(workerSystems);

            var leader = seedSystems.FirstOrDefault() ?? throw new CannotDetermineLeaderException();

            TaskCompletionSource<bool> clusterReady = new TaskCompletionSource<bool>(false);

            var akkaCluster = Akka.Cluster.Cluster.Get(leader);
            akkaCluster.RegisterOnMemberUp(() =>
                                           {
                                               foreach (var systemBuilder in allActorSystems)
                                               {
                                                   PerformOnMemberUp(systemBuilder).Wait();
                                               }

                                               clusterReady.SetResult(true);
                                           });

            foreach (var address in autoSeedAddresses)
                await akkaCluster.JoinSeedNodesAsync(new[] {address});

            foreach (var address in workerSystemAddresses)
                await akkaCluster.JoinAsync(address);

            await clusterReady.Task;

            return new ClusterInfo(akkaCluster,
                                   seedSystemAddresses.Concat(autoSeedAddresses)
                                                      .Concat(workerSystemAddresses)
                                                      .ToArray());
        }

        private async Task<ActorSystem[]> CreateSystems(IReadOnlyCollection<IActorSystemConfigBuilder> actorSystemBuilders, Func<ActorSystem, Task> init)
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