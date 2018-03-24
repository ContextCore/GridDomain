using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Routing;
using Autofac;
using GridDomain.Common;
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

    public class ClusterCommandPipe : IMessagesRouter
    {
        private Akka.Cluster.Cluster _cluster;
        private readonly ExtendedActorSystem _system;
        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }

        public ClusterCommandPipe(Akka.Cluster.Cluster cluster, ILogger log)
        {
            _log = log;
            _cluster = cluster;
            _system = _cluster.System;
        }

        public Task<IActorRef> Init(ContainerBuilder container)
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

            CommandExecutor = _system.ActorOf(Props.Empty.WithRouter(routingGroup));

            _log.Debug("Command pipe is starting");

            ProcessesPipeActor = _system.ActorOf(Props.Create(() => new DummyProcessActor()), nameof(Actors.CommandPipe.ProcessesPipeActor));

            HandlersPipeActor = _system.ActorOf(Props.Create(() => new DummyHandlersActor()),
                                                nameof(Actors.CommandPipe.HandlersPipeActor));

            container.RegisterInstance(HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
            
            container.RegisterInstance(ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);

            return Task.FromResult(CommandExecutor);
        }

        readonly List<string> _shardRegionPaths = new List<string>();
        private readonly ILogger _log;

        public async Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var actorType = typeof(AggregateActorCell<>).MakeGenericType(descriptor.AggregateType);

            var region = await ClusterSharding.Get(_system)
                                              .StartAsync(descriptor.AggregateType.BeautyName(),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(_system),
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
    }
}