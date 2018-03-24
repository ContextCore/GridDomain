using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
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
    }
}