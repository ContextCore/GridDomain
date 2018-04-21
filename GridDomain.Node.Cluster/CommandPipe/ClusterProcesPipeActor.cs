using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class ClusterProcesPipeActor : ProcessesPipeActor, IWithUnboundedStash
    {

        public ClusterProcesPipeActor( IReadOnlyCollection<IProcessDescriptor> processDescriptors,string commandActorPath) : base(CreateRoutes(Context, processDescriptors))
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

        public class ShardedProcessMessageProcessor : IMessageProcessor<IProcessCompleted>
        {
            private readonly IActorRef _processRegion;
            private readonly TimeSpan? _defaultTimeout;
            private readonly string _processStateName;

            public ShardedProcessMessageProcessor(IActorRef processRegion,
                                                  string processStateName,
                                                  TimeSpan? defaultTimeout = null)
            {
                _processStateName = processStateName;
                _defaultTimeout = defaultTimeout;
                _processRegion = processRegion;
            }

            public Task<IProcessCompleted> Process(IMessageMetadataEnvelop message)
            {
                return _processRegion.Ask<IProcessCompleted>(MessageFactory(message), _defaultTimeout);
            }

            private IMessageMetadataEnvelop MessageFactory(IMessageMetadataEnvelop m)
            {
                if (m.Message is IHaveProcessId proc)
                {
                    return new ShardedProcessMessageMetadataEnvelop(
                                                                    m.Message,
                                                                    proc.ProcessId,
                                                                    _processStateName,
                                                                    m.Metadata);
                }

                throw new InvalidMessageException();
            }

            Task IMessageProcessor.Process(IMessageMetadataEnvelop message)
            {
                return Process(message);
            }
        }

        private static IMessageProcessor<ProcessesTransitComplete> CreateRoutes(IUntypedActorContext context, IReadOnlyCollection<IProcessDescriptor> messageRouteMap)
        {
            var catalog = new ProcessesDefaultProcessor();
            foreach (var reg in messageRouteMap)
            {
                var procesShardRegion = ClusterSharding.Get(context.System)
                                                       .ShardRegion(Known.Names.Region(reg.ProcessType));

                var processStateName = reg.StateType.BeautyName();

                IMessageProcessor<IProcessCompleted> processor = new ShardedProcessMessageProcessor(procesShardRegion,
                                                                                                    processStateName);
                foreach(var msg in reg.AcceptMessages)
                    catalog.Add(msg.MessageType, processor);
            }

            return catalog;
        }

//        private static IMessageProcessor<ProcessesTransitComplete> CreateRoutess(IUntypedActorContext context, MessageMap messageRouteMap)
//        {
//            var catalog = new ProcessesDefaultProcessor();
//            foreach (var reg in messageRouteMap.Registratios)
//            {
//                var procesShardRegion = ClusterSharding.Get(context.System)
//                                                       .ShardRegion(Known.Names.Region(reg.Handler));
//
//                IMessageProcessor<IProcessCompleted> processor;
//                switch (reg.ProcesType)
//                {
//                    case MessageMap.HandlerProcessType.Sync:
//                        processor = new ShardedProcessMessageProcessor<IProcessCompleted>(procesShardRegion);
//                        break;
//
//                    case MessageMap.HandlerProcessType.FireAndForget:
//                        processor = new FireAndForgetActorMessageProcessor<IProcessCompleted>(procesShardRegion, new ProcessTransited(null, null, null, null));
//                        break;
//                    default:
//                        throw new NotSupportedException(reg.ProcesType.ToString());
//                }
//
//                catalog.Add(reg.Message, processor);
//            }
//
//            return catalog;
//        }

        public IStash Stash { get; set; }
    }
}