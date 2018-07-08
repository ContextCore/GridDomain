using System;
using GridDomain.Common;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ClusterGetProcessState : GetProcessState, IShardedMessageMetadataEnvelop
    {
        public ClusterGetProcessState(Type type, string id, IShardIdGenerator generator = null) : base(id)
        {
            Message = this;
            Metadata = MessageMetadata.Empty;
            EntityId = EntityActorName.GetFullName(type,id);
            ShardId = (generator ?? DefaultShardIdGenerator.Instance).GetShardId(id);
        }

        public object Message { get; }
        public IMessageMetadata Metadata { get; }
        public string EntityId { get; }
        public string ShardId { get; }
    }
}