using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Routing;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.ProcessManagers.DomainBind;
using Serilog;

namespace GridDomain.Node.Cluster
{

    public static class GridNodeBuilderExtensions
    {
        public static IGridDomainNode BuildCluster(this GridNodeBuilder builder)
        {
             return new GridClusterNode(builder.Configurations,builder.ActorCommandPipeFactory,builder.Logger,builder.DefaultTimeout);
        }
    }
    
    public class GridClusterNode : GridDomainNode
    {
        public GridClusterNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        protected override ICommandExecutor CreateCommandExecutor()
        {
            return new ClusterCommandExecutor(System,Transport,DefaultTimeout);
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new ClusterCommandPipe(System,Log);
        }
    }

    
    internal class DummyProcessActor : ReceiveActor
    {
        public DummyProcessActor()
        {
            Receive<IMessageMetadataEnvelop<DomainEvent>>(
                                                          m => Sender.Tell(new ProcessTransited(null, null, null, null)));
        }
    }

    public class DummyHandlersActor : ReceiveActor
    {
        public DummyHandlersActor()
        {
            Receive<IMessageMetadataEnvelop>(envelop => Sender.Tell(AllHandlersCompleted.Instance));
            Receive<ProcessesTransitComplete>(t => { });
        }
    }

    public class ClusterCommandPipe : IActorCommandPipe
    {
        public  ActorSystem System { get; }
        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }

        public ClusterCommandPipe(ActorSystem cluster, ILogger log)
        {
            _log = log;
            System = cluster;
            
            ProcessesPipeActor = System.ActorOf(Props.Create(() => new DummyProcessActor()), nameof(Actors.CommandPipe.ProcessesPipeActor));

            HandlersPipeActor = System.ActorOf(Props.Create(() => new DummyHandlersActor()), nameof(Actors.CommandPipe.HandlersPipeActor));

        }

        readonly List<string> _shardRegionPaths = new List<string>();
        private readonly ILogger _log;

        public async Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var actorType = typeof(AggregateActorCell<>).MakeGenericType(descriptor.AggregateType);

            var region = await ClusterSharding.Get(System)
                                              .StartAsync(descriptor.AggregateType.BeautyName(),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(System),
                                                          new ShardedMessageMetadataExtractor());
            _shardRegionPaths.Add(region.Path.ToString());
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            throw new NotImplementedException();
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            throw new NotImplementedException();
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            throw new NotImplementedException();
        }

        public Task BuildRoutes()
        {
            var routingGroup = new ConsistentHashingGroup(_shardRegionPaths)
                .WithHashMapping(m =>
                                 {
                                     if (m is IShardedMessageMetadataEnvelop env && env.Message is ICommand cmd)
                                     {
                                         return cmd.AggregateType;
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            CommandExecutor = System.ActorOf(Props.Empty.WithRouter(routingGroup));
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterInstance(HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
            
            container.RegisterInstance(ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);
        }
    }
}