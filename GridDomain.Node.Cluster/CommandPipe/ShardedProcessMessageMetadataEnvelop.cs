using System;
using Akka.TestKit;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Cluster.CommandPipe {
    
    public class ShardedProcessMessageMetadataEnvelop : IShardedMessageMetadataEnvelop
    {
        public ShardedProcessMessageMetadataEnvelop(object evt, string processId, string processStateName, IMessageMetadata metadata=null, IShardIdGenerator generator=null)
        {
            Message = evt;
            Metadata = metadata ?? MessageMetadata.Empty;
            generator = generator ?? DefaultShardIdGenerator.Instance;
            
            ShardId = generator.GetShardId(processId);
            EntityId = EntityActorName.GetFullName(processStateName,processId);
        }

        public string EntityId { get; }
        public string ShardId { get; }
        public object Message { get; }
        public IMessageMetadata Metadata { get; }
    }
}