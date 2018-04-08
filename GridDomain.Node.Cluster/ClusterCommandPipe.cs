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
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node.Cluster
{

    public class ClusterMessageWaiterFactory : IMessageWaiterFactory
    {
        public ClusterMessageWaiterFactory(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout)
        {
            System = system;
            DefaultTimeout = defaultTimeout;
            Transport = transport;
        }

        public IActorTransport Transport { get; }
        public TimeSpan DefaultTimeout { get; }
        public ActorSystem System { get; }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<Task<IWaitResult>>();
            var waiter = new MessagesWaiter<Task<IWaitResult>>(System, Transport, defaultTimeout ?? DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = waiter.Start;
            return waiter;
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
           throw new NotSupportedException("Cluster cannot wait explicit messages due to topic-based pub-sub");
        }
    }
    
    public static class GridNodeBuilderExtensions
    {
        public static IGridDomainNode BuildCluster(this GridNodeBuilder builder)
        {
             return new GridClusterNode(builder.Configurations,builder.ActorSystemFactory,builder.Logger,builder.DefaultTimeout);
        }
    }
    
    public class GridClusterNode : GridDomainNode
    {
        private ClusterCommandExecutor _clusterCommandExecutor;
        public GridClusterNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        protected override ICommandExecutor CreateCommandExecutor()
        {
            _clusterCommandExecutor = new ClusterCommandExecutor(System,Transport,DefaultTimeout);
            return _clusterCommandExecutor;
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new ClusterCommandPipe(System,Log);
        }

        protected override async Task ConfigurePipe(DomainBuilder domainBuilder)
        {
            await base.ConfigurePipe(domainBuilder);
            _clusterCommandExecutor.Init(Pipe.CommandExecutor);
        }

        protected override IActorTransport CreateTransport()
        {
           var ext =  System.InitDistributedTransport();
           return ext.Transport;
        }
        protected override IMessageWaiterFactory CreateMessageWaiterFactory()
        {
            return new ClusterMessageWaiterFactory(System, Transport, DefaultTimeout);
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
            var path = region.Path.ToString();

            if(_shardRegionPaths.Contains(path))
                throw new InvalidOperationException("Cannot add dublicate region path: " + path);  
            
            _shardRegionPaths.Add(path);
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            return Task.CompletedTask;
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return Task.CompletedTask;
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return Task.CompletedTask;
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