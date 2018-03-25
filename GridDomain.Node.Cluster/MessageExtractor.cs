using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Cluster
{


    public sealed class ShardedMessageMetadataExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as IShardedMessageMetadataEnvelop)?.EntityId;

        public string ShardId(object message) => (message as IShardedMessageMetadataEnvelop)?.ShardId;

        public object EntityMessage(object message) => (message as IMessageMetadataEnvelop);
    }

    /// <summary>
    /// Generates shardId as string from given seed and max shards number
    /// ca be used to change names generation algorithm for shards
    /// </summary>
    public interface IShardIdGenerator
    {
        string Resolve(string seed, int? maxShards=null);
    }

    public class DefaultShardIdGenerator : IShardIdGenerator
    {
        private readonly string _shardGroupName;
        private readonly int _maxShards;

        public DefaultShardIdGenerator(string shardGroupName,int maxShards = 5)
        {
            _maxShards = maxShards;
            _shardGroupName = shardGroupName;
        }
        public static IShardIdGenerator Instance = new DefaultShardIdGenerator("");
        public string Resolve(string seed, int? maxShards=null)
        {
            var seedNumber = seed.Aggregate(0,
                                            (s, c) =>
                                            {
                                                unchecked
                                                {
                                                    return s + (int) c;
                                                }
                                            });
            //will return same shard for same pair (AggregateId, MaxShardsId)
            //randomly distributed between values (1 .. MaxShardId)
            //TODO: may be it is better to cache random per thread?
            var variationPart = new Random(seedNumber).Next(maxShards ?? _maxShards)
                                                      .ToString();
            return _shardGroupName + "_" + variationPart;
        }
    }

    public class ShardedCommandMetadataEnvelop : IShardedMessageMetadataEnvelop
    {
        private readonly IHaveId _command;
        public object Message => _command;

        public IMessageMetadata Metadata { get; }
        public string EntityId { get; }
        public string ShardId { get; }

        public ShardedCommandMetadataEnvelop(ICommand message, IMessageMetadata metadata = null, IShardIdGenerator generator=null)
        {
            _command = message;
            Metadata = metadata ?? MessageMetadata.Empty;
            generator = generator ?? DefaultShardIdGenerator.Instance;
            ShardId = generator.Resolve(message.AggregateId);
            EntityId = EntityActorName.GetFullName(message.AggregateType, message.AggregateId);
        }
    }
}