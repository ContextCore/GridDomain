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
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.Transport.Extension;
using Serilog;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class ClusterHandlersPipeActor : HandlersPipeActor
    {
        public ClusterHandlersPipeActor(MessageMap map, IActorRef processActor) : base(CreateRoutess(Context, map), processActor) { }

        private static IMessageProcessor CreateRoutess(IUntypedActorContext system, MessageMap messageRouteMap)
        {
            var catalog = new HandlersDefaultProcessor();
            foreach (var reg in messageRouteMap.Registratios)
            {
                var handlerActorType = typeof(MessageHandleActor<,>).MakeGenericType(reg.Message, reg.Handler);

                var props = system.DI()
                                  .Props(handlerActorType);

                var actor = system.ActorOf(props, handlerActorType.BeautyName());

                IMessageProcessor processor;
                switch (reg.ProcesType)
                {
                    case MessageMap.HandlerProcessType.Sync:
                        processor = new ActorAskMessageProcessor<HandlerExecuted>(actor);
                        break;

                    case MessageMap.HandlerProcessType.FireAndForget:
                        processor = new FireAndForgetActorMessageProcessor(actor);
                        break;
                    default:
                        throw new NotSupportedException(reg.ProcesType.ToString());
                }

                catalog.Add(reg.Message, processor);
            }

            return catalog;
        }
    }

    public class MessageMap
    {
        public enum HandlerProcessType
        {
            Sync,
            FireAndForget
        }

        public class HandlerRegistration
        {
            public HandlerRegistration(Type message, Type handler, HandlerProcessType procesType)
            {
                Message = message;
                Handler = handler;
                ProcesType = procesType;
            }

            public Type Message { get; }
            public Type Handler { get; }
            public HandlerProcessType ProcesType { get; }
        }

        public List<HandlerRegistration> Registratios = new List<HandlerRegistration>();

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            Registratios.Add(new HandlerRegistration(typeof(TMessage), typeof(THandler), HandlerProcessType.Sync));
            return Task.CompletedTask;
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            Registratios.Add(new HandlerRegistration(typeof(TMessage), typeof(THandler), HandlerProcessType.FireAndForget));
            return Task.CompletedTask;
        }
    }

    public class ClusterCommandPipe : IActorCommandPipe
    {
        public ActorSystem System { get; }
        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }
        private readonly ICompositeMessageProcessor _handlersCatalog;

        readonly List<string> _aggregatesRegionPaths = new List<string>();

        //readonly List<string> _messageHandlersShards = new List<string>();
        MessageMap messageMap = new MessageMap();

        private readonly ILogger _log;
        //private readonly List<Func<Task>> _postponedRegistrations = new List<Func<Task>>();

        public ClusterCommandPipe(ActorSystem cluster, ILogger log)
        {
            _log = log;
            System = cluster;
            _handlersCatalog = new HandlersDefaultProcessor();

            ProcessesPipeActor = System.ActorOf(Props.Create(() => new DummyProcessActor()), nameof(Actors.CommandPipe.ProcessesPipeActor));
        }

        public async Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var actorType = typeof(AggregateActorCell<>).MakeGenericType(descriptor.AggregateType);

            var region = await ClusterSharding.Get(System)
                                              .StartAsync(descriptor.AggregateType.BeautyName(),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(System),
                                                          new ShardedMessageMetadataExtractor());
            var path = region.Path.ToString();

            if (_aggregatesRegionPaths.Contains(path))
                throw new InvalidOperationException("Cannot add dublicate region path: " + path);

            _aggregatesRegionPaths.Add(path);
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            return Task.CompletedTask;
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return messageMap.RegisterSyncHandler<TMessage, THandler>();
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return messageMap.RegisterFireAndForgetHandler<TMessage, THandler>();
        }

        public async Task StartRoutes()
        {
            BuildMessageHandlers();

            BuildAggregateCommandingRoutes();

            await Task.CompletedTask;
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
            var routingGroup = new ConsistentHashingGroup(_aggregatesRegionPaths)
                .WithHashMapping(m =>
                                 {
                                     if (m is IShardedMessageMetadataEnvelop env && env.Message is ICommand cmd)
                                     {
                                         return cmd.AggregateType;
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            CommandExecutor = System.ActorOf(Props.Empty.WithRouter(routingGroup));
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterType<ClusterHandlersPipeActor>()
                     .WithParameters(new Parameter[]
                                     {
                                         new TypedParameter(typeof(MessageMap), messageMap),
                                         new TypedParameter(typeof(IActorRef), ProcessesPipeActor)
                                     });

            container.RegisterInstance(ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);

            container.Register((c, p) => HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
        }
    }
}