using System;
using System.Security.Cryptography.X509Certificates;
using GridDomain.Node.Akka.Actors;

namespace GridDomain.Node
{
    public class ShardEnvelop : IShardEnvelop
    {
        public string EntityId { get; protected set; }
        public string ShardId { get; protected set; }
        public string Region { get; }
        public object Message { get; protected set; }

        public ShardEnvelop(object message, string shardId, string entityId, string region)
        {
            Message = message;
            ShardId = shardId;
            EntityId = entityId;
            Region = region;
        }
    }
}