using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Cluster.CommandPipe {



    public class ClusterProcessActor<T> : ProcessActor<T> where T : class, IProcessState
    {
        private IActorRef _shardRegion;

        public ClusterProcessActor(IProcess<T> process, IProcessStateFactory<T> processStateFactory, string stateActorPath) : base(process, processStateFactory, stateActorPath)
        {
            _shardRegion = ClusterSharding.Get(Context.System)
                                          .ShardRegion(Known.Names.Region(process.GetType()));
        }
        protected override object CreateGetStateMessage()
        {
            return new ClusterGetProcessState(typeof(T),Id);
        }

        protected override IMessageMetadataEnvelop EnvelopStateCommand(ICommand cmd, IMessageMetadata metadata)
        {
            return new ShardedCommandMetadataEnvelop(cmd,metadata);
        }

        protected override IActorRef RedirectActor()
        {
            return _shardRegion;
        }

        protected override IMessageMetadataEnvelop EnvelopRedirect(ProcessRedirect processRedirect, IMessageMetadata metadata)
        {
            return new ShardedProcessMessageMetadataEnvelop(processRedirect, processRedirect.ProcessId,metadata);
        }
    }

    public class ClusterGetProcessState:GetProcessState,IShardedMessageMetadataEnvelop
    {
        public ClusterGetProcessState(Type type, string id, IShardIdGenerator generator = null):base(id)
        {
            Message = this;
            Metadata = MessageMetadata.Empty;
            EntityId = id;
            ShardId = (generator ?? DefaultShardIdGenerator.Instance).GetShardId(id);
        }

        public object Message { get; }
        public IMessageMetadata Metadata { get; }
        public string EntityId { get; }
        public string ShardId { get; }
    }

    public class ClusterProcesPipeActor : ProcessesPipeActor, IWithUnboundedStash
    {
        public ClusterProcesPipeActor(MessageMap map, string commandActorPath) : base(CreateRoutess(Context, map))
        {
            //initialize command actor on first message, 
            //switch to default behavior after
            BecomeStacked(() => AwaitingAnyMessageBehavior(commandActorPath));
        }

        private void AwaitingAnyMessageBehavior(string commandActorPath)
        {
            ReceiveAny(m =>
                       {
                           Context.ActorSelection(commandActorPath)
                                  .ResolveOne(TimeSpan.FromSeconds(5))
                                  .PipeTo(Self);
                           
                           BecomeStacked(SetCommandActorBehavior);
                           
                           Stash.Stash();
                       });  
        }

        private void SetCommandActorBehavior()
        {
            Receive<IActorRef>(m =>
                               {
                                   _commandExecutionActor = m;
                                   //return to default behavior of parent actor
                                   UnbecomeStacked();
                                   UnbecomeStacked();
                                   Stash.UnstashAll();
                               });
            Receive<Status.Failure>(f => throw new InvalidOperationException());
            ReceiveAny(m => Stash.Stash());
        }

        private class FireAndForgetActorMessageProcessor<T> : IMessageProcessor<T>
        {
            private readonly T _answer;

            public FireAndForgetActorMessageProcessor(IActorRef processor, T answer)
            {
                _answer = answer;
                ActorRef = processor;
            }

            public Task<T> Process(IMessageMetadataEnvelop message)
            {
                ActorRef.Tell(message);
                return Task.FromResult(_answer);
            }

            private IActorRef ActorRef { get; }

            Task IMessageProcessor.Process(IMessageMetadataEnvelop message)
            {
                return Process(message);
            }
        }

        private static IMessageProcessor<ProcessesTransitComplete> CreateRoutess(IUntypedActorContext context, MessageMap messageRouteMap)
        {
            var catalog = new ProcessesDefaultProcessor();
            foreach (var reg in messageRouteMap.Registratios)
            {
                var procesShardRegion = ClusterSharding.Get(context.System)
                                                       .ShardRegion(Known.Names.Region(reg.Handler));

                IMessageProcessor<IProcessCompleted> processor;
                switch (reg.ProcesType)
                {
                    case MessageMap.HandlerProcessType.Sync:
                        processor = new ActorAskMessageProcessor<IProcessCompleted>(procesShardRegion);
                        break;

                    case MessageMap.HandlerProcessType.FireAndForget:
                        processor = new FireAndForgetActorMessageProcessor<IProcessCompleted>(procesShardRegion, new ProcessTransited(null, null, null, null));
                        break;
                    default:
                        throw new NotSupportedException(reg.ProcesType.ToString());
                }

                catalog.Add(reg.Message, processor);
            }

            return catalog;
        }

        public IStash Stash { get; set; }
    }
}