using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Transport;

namespace GridDomain.Node.Cluster {
    
    public class ClusterCommandExecutor : AkkaCommandExecutor
    {
        public ClusterCommandExecutor(ActorSystem system, IActorTransport transport, TimeSpan defaultTimeout) : base(system, transport, defaultTimeout)
        {
            
        }

        protected override IMessageMetadataEnvelop EnvelopeCommand<T>(T command, IMessageMetadata metadata)
        {
            return new ShardedCommandMetadataEnvelop(command, metadata ?? CreateEmptyCommandMetadata(command));
        }
        public override ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null)  
        {
            return new CommandWaiter<T>(
                                        _system,
                                        _transport,
                                        _defaultTimeout,
                                        new CommandConditionBuilder<T>(cmd, 
                                                                       metadata,
                                                                       this,
                                                                       new ClusterCorrelationConditionBuilder<Task<IWaitResult>>(metadata?.CorrelationId))
                                        );
        }
    }

    public class ClusterCorrelationConditionBuilder<T> : CorrelationConditionBuilder<T>
    {
        public ClusterCorrelationConditionBuilder(string correlationId) : base(correlationId) { }
    }
    
    
}