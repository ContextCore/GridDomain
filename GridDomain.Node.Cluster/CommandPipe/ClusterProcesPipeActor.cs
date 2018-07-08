using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Event;
using Akka.IO;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.ProcessManagers.DomainBind;
using Serilog;

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
                                   CommandExecutionActor = m;
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
            private readonly ILoggingAdapter _log;
            private const string EmptyProcessId = "0x";

            public ShardedProcessMessageProcessor(IActorRef processRegion,
                                                  string processStateName,
                                                  ILoggingAdapter log,
                                                  TimeSpan? defaultTimeout = null)
            {
                _log = log;
                _processStateName = processStateName;
                _defaultTimeout = defaultTimeout;
                _processRegion = processRegion;
            }

            public Task<IProcessCompleted> Process(IMessageMetadataEnvelop message)
            {
                var shardedMessage = MessageFactory(message);
                _log.Debug("Sending message {@m} to sharded process managers", shardedMessage);
                return _processRegion.Ask<IProcessCompleted>(shardedMessage, _defaultTimeout);
            }

            private IMessageMetadataEnvelop MessageFactory(IMessageMetadataEnvelop m)
            {
                string processId = null;

                switch (m.Message) {
                    case IHaveProcessId proc:
                        processId = proc.ProcessId ?? EmptyProcessId;
                        break;
                    case IFault f:
                        processId = f.ProcessId ?? EmptyProcessId;
                        break;
                }

                if (processId != null)
                {
                    var shardedProcessMessageMetadataEnvelop = new ShardedProcessMessageMetadataEnvelop(
                                                                                                        m.Message,
                                                                                                        processId,
                                                                                                        _processStateName,
                                                                                                        m.Metadata);

                    return shardedProcessMessageMetadataEnvelop;
                }

                var invalidMessageException = new CannotExtractProcessIdException();
                _log.Error(invalidMessageException,"cannot extract process id from message received {@m}",m);

                throw invalidMessageException;
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
                                                                                                    processStateName,
                                                                                                    Context.GetSeriLogger());
                foreach(var msg in reg.AcceptMessages)
                    catalog.Add(msg.MessageType, processor);
            }

            return catalog;
        }


        public IStash Stash { get; set; }
        public override IMessageMetadataEnvelop EnvelopCommand(ICommand cmd, IMessageMetadataEnvelop initialMessage)
        {
            return new ShardedCommandMetadataEnvelop(cmd,initialMessage.Metadata.CreateChild(cmd));
        }
    }
}