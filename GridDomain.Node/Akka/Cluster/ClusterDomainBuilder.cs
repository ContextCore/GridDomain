using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Autofac;
using GridDomain.Aggregates;
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

            var region = await clusterSharding.StartAsync(regionName,
                aggregateProps,
                ClusterShardingSettings.Create(_system),
                new ShardedMessageMetadataExtractor()); 

            _aggregatesRegions.Add(regionName, region);
        }

        public Task<IDomain> Build()
        {
            var container = _containerBuilder.Build();
            _system.InitAggregatesExtension(container);

            return BuildCommandExecutor();
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

            return Task.FromResult<IDomain>(new Domain(new ActorCommandExecutor(commandActor),
                new ClusterAggregatesController(_system, commandActor)));
        }

        public class UnknownShardMessageException : Exception
        {
        }

        public class CannotFindRequestedRegion : Exception
        {
        }
    }
}