using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Domains;
using GridDomain.Node.Akka.Actors;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.AggregatesExtension;
using GridDomain.Node.Akka.Cluster.CommandGrouping;

namespace GridDomain.Node.Akka.Cluster
{
    
    public class ClusterDomainBuilder : IDomainBuilder
    {
        private readonly ActorSystem _system;
        readonly Dictionary<string, IActorRef> _aggregatesRegions = new Dictionary<string, IActorRef>();
        private Dictionary<Type, object> _commandHandlerProxies = new Dictionary<Type,object>();
        private readonly ContainerBuilder _containerBuilder;

        private static readonly OneForOneStrategy _supervisorStrategy
            = new OneForOneStrategy(ex =>
            {
                switch (ex)
                {
                    case AggregateActor.CommandExecutionException cf:
                        return Directive.Restart;
                    case CommandAlreadyExecutedException cae:
                        return Directive.Restart;
                    default:
                        return Directive.Stop;
                }
            });

        public ClusterDomainBuilder(ActorSystem system, ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
            _system = system;
        }

        public async Task RegisterAggregate<TAggregate>(IAggregateConfiguration<TAggregate> configuration)
            where TAggregate : class, IAggregate
        {
            _containerBuilder.RegisterInstance(configuration);


            var clusterSharding = ClusterSharding.Get(_system);
            var regionName = AggregateAddress.New<TAggregate>("regionOnly").Name;

            if (_aggregatesRegions.ContainsKey(regionName))
                throw new InvalidOperationException("Cannot add duplicate region with name: " + regionName);

            var aggregateProps = Props.Create<AggregateActor<TAggregate>>()
                .WithSupervisorStrategy(_supervisorStrategy);

            _system.Log.Info("Starting new shard region {regionName}", regionName);

            var setting = ClusterShardingSettings.Create(_system);
            if (!string.IsNullOrEmpty(configuration.Settings.HostRole))
                setting = setting.WithRole(configuration.Settings.HostRole);
            
            var region = await clusterSharding.StartAsync(regionName,
                aggregateProps,setting,
                new ShardedMessageMetadataExtractor()); 

            _aggregatesRegions.Add(regionName, region);
        }

        public Task<IDomain> Build()
        {
            var container = _containerBuilder.Build();
            _system.InitAggregatesExtension(container);

            return BuildCommandExecutor();
        }

        public void RegisterCommandHandler<T>(Func<ICommandHandler<ICommand>, T> proxyBuilder)
        {
            _commandHandlerProxies[typeof(T)] = proxyBuilder;
        }

        public void RegisterCommandsResultAdapter<TAggregate>(ICommandsResultAdapter adapter) where TAggregate : IAggregate
        {
            _containerBuilder.RegisterInstance(adapter).Named<ICommandsResultAdapter>(typeof(TAggregate).BeautyName());
        }

        private Task<IDomain> BuildCommandExecutor()
        {
            var routingGroup = new ConsistentMapGroup(_aggregatesRegions)
                .WithMapping(m =>
                {
                    if (!(m is IShardEnvelop env)) throw new UnknownShardMessageException();
                    if (!_aggregatesRegions.ContainsKey(env.Region)) throw new CannotFindRequestedRegion();
                    return env.Region;
                });


            var commandActor = _system.ActorOf(Props.Empty.WithRouter(routingGroup), "Aggregates");

            return Task.FromResult<IDomain>(new Domain(new ActorCommandExecutor(commandActor, _system.Log),
                new ClusterAggregatesController(_system, commandActor),_commandHandlerProxies));
        }

        public class UnknownShardMessageException : Exception
        {
        }

        public class CannotFindRequestedRegion : Exception
        {
        }
    }
}