using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Cluster.MessageWaiting;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster.CommandPipe {
    
    public class ClusterCommandExecutor : AkkaCommandExecutor
    {
        public ClusterCommandExecutor(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout) : base(system, transport, defaultTimeout)
        {
            
        }

        protected override IMessageMetadataEnvelop EnvelopeCommand<T>(T command, IMessageMetadata metadata)
        {
            if(metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            return new ShardedCommandMetadataEnvelop(command, metadata);
        }
        
        public override ICommandExpectationBuilder Prepare<T>(T cmd, IMessageMetadata metadata = null)
        {
            metadata = metadata ?? CreateEmptyCommandMetadata(cmd);
            
            return new CommandExpectationBuilder<T>(
                                        _system,
                                        _transport,
                                        _defaultTimeout,
                                        new ConditionCommandExecutor<T>(cmd, 
                                                                       metadata,
                                                                       this,
                                                                       new ClusterCorrelationConditionFactory<Task<IWaitResult>>(metadata.CorrelationId))
                                        );
        }
    }
}