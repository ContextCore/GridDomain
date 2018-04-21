using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Security;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using Akka.Event;
using Akka.Routing;
using Akka.Util.Internal;
using Autofac;
using Autofac.Core;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Cluster.CommandPipe.CommandGrouping;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;
using GridDomain.Transport.Extension;
using Serilog;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class ClusterCommandPipe : IActorCommandPipe
    {
        public ActorSystem System { get; }
        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }

        readonly Dictionary<string, IActorRef> _aggregatesRegions = new Dictionary<string, IActorRef>();
        readonly List<string> _processAggregatesRegionPaths = new List<string>();

        readonly MessageMap _messageMap = new MessageMap();

        private readonly ILogger _log;
        private readonly MessageMap _processMessageMap = new MessageMap();

        public ClusterCommandPipe(ActorSystem system, ILogger log)
        {
            _log = log;
            System = system;
        }

        public async Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            await RegisterAggregateByType(descriptor.AggregateType);
        }

        private async Task RegisterAggregateByType(Type aggregateType)
        {
            var actorType = typeof(AggregateActorCell<>).MakeGenericType(aggregateType);

            var region = await ClusterSharding.Get(System)
                                              .StartAsync(Known.Names.Region(aggregateType),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(System),
                                                          new ShardedMessageMetadataExtractor());

            if (_aggregatesRegions.ContainsKey(Known.Names.Region(aggregateType)))
                throw new InvalidOperationException("Cannot add dublicate region path: " + region.Path);

            _aggregatesRegions.Add(Known.Names.Region(aggregateType), region);
        }

        public async Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            await RegisterAggregateByType(typeof(ProcessStateAggregate<>).MakeGenericType(processDescriptor.StateType));

            var actorType = typeof(ProcessActorCell<>).MakeGenericType(processDescriptor.StateType);

            var region = await ClusterSharding.Get(System)
                                              .StartAsync(Known.Names.Region(processDescriptor.ProcessType),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(System),
                                                          new ShardedMessageMetadataExtractor());
            var path = region.Path.ToString();

            if (_processAggregatesRegionPaths.Contains(path))
                throw new InvalidOperationException("Cannot add dublicate region path: " + path);


            // _processAggregatesRegionPaths.Add(path);
            foreach (var msg in processDescriptor.AcceptMessages)
                _processMessageMap.Registratios.Add(new MessageMap.HandlerRegistration(msg.MessageType, processDescriptor.ProcessType, MessageMap.HandlerProcessType.Sync));
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return _messageMap.RegisterSync<TMessage, THandler>();
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return _messageMap.RegisterFireAndForget<TMessage, THandler>();
        }

        public async Task StartRoutes()
        {
            BuildProcessManagerRoutes();

            BuildMessageHandlers();

            BuildAggregateCommandingRoutes();

            await Task.CompletedTask;
        }

        private void BuildProcessManagerRoutes()
        {
            var routingGroup = new ConsistentHashingPool(10)
                .WithHashMapping(m =>
                                 {
                                     if (m is IMessageMetadataEnvelop env && env.Message is DomainEvent evt)
                                     {
                                         return evt.SourceId;
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            var processesProps = System.DI()
                                       .Props(typeof(ClusterProcesPipeActor));

            ProcessesPipeActor = System.ActorOf(processesProps.WithRouter(routingGroup), "Processes");
        }

        private void BuildMessageHandlers()
        {
            var routingPool = new ConsistentHashingPool(10)
                .WithHashMapping(m =>
                                 {
                                     if (m is IMessageMetadataEnvelop env)
                                     {
                                         return env.Metadata.CorrelationId;
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            var clusterRouterPool = new ClusterRouterPool(routingPool, new ClusterRouterPoolSettings(10, 1, true));

            var handlerActorProps = System.DI()
                                          .Props(typeof(ClusterHandlersPipeActor))
                                          .WithRouter(clusterRouterPool);

            HandlersPipeActor = System.ActorOf(handlerActorProps, nameof(ClusterHandlersPipeActor));
        }

        private void BuildAggregateCommandingRoutes()
        {
            var routingGroup = new ConsistentMapGroup(_aggregatesRegions)
                .WithMapping(m =>
                             {
                                 if (m is IShardedMessageMetadataEnvelop env && env.Message is ICommand cmd)
                                 {
                                     return cmd.AggregateType;
                                 }

                                 throw new InvalidMessageException(m.ToString());
                             });

            CommandExecutor = System.ActorOf(Props.Empty.WithRouter(routingGroup), "Aggregates");
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterType<ClusterProcesPipeActor>()
                     .WithParameters(new Parameter[]
                                     {
                                         new TypedParameter(typeof(MessageMap), _processMessageMap),
                                         //{akka://AutoTest/user/Aggregates}
                                         new TypedParameter(typeof(string), "/user/Aggregates")
                                     });

            container.RegisterType<ClusterHandlersPipeActor>()
                     .WithParameters(new Parameter[]
                                     {
                                         new TypedParameter(typeof(MessageMap), _messageMap),
                                         new TypedParameter(typeof(IActorRef), ProcessesPipeActor)
                                     });

            container.Register((c, p) => ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);

            container.Register((c, p) => HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
        }
    }
}