using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;

namespace GridDomain.Node.Cluster
{
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as ShardEnvelope)?.EntityId;

        public string ShardId(object message) => (message as ShardEnvelope)?.ShardId;

        public object EntityMessage(object message) => (message as ShardEnvelope)?.Message;
    }

    public sealed class ShardedMessageMetadataExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as IShardedMessageMetadataEnvelop)?.Message?.Id;

        public string ShardId(object message) => (message as IShardedMessageMetadataEnvelop)?.ShardId;

        public object EntityMessage(object message) => (message as IMessageMetadataEnvelop);
    }

    public class ShardedCommandMetadataEnvelop : IShardedMessageMetadataEnvelop<ICommand>
    {
        object IMessageMetadataEnvelop.Message => Message;

        public ICommand Message { get; }
        public IMessageMetadata Metadata { get; }
        public string ShardId { get; }

        public ShardedCommandMetadataEnvelop(ICommand message, int maxShards = 5, IMessageMetadata metadata = null)
        {
            Message = message;
            Metadata = metadata ?? MessageMetadata.Empty;
            var seed = message.AggregateId.Aggregate(0,
                                                     (s, c) =>
                                                     {
                                                         unchecked
                                                         {
                                                             return s + (int) c;
                                                         }
                                                     });
            //will return same shard for same pair (AggregateId, MaxShardsId)
            //randomly distributed between values (0 .. MaxShardId)
            ShardId = new Random(seed).Next(maxShards)
                                      .ToString();
        }
    }

    public class CommandClusterExecutor //: ICommandExecutor
    {
        private readonly ActorSystem _actorSystem;

        public CommandClusterExecutor(ActorSystem system)
        {
            _actorSystem = system;
            _clusterSharding = ClusterSharding.Get(_actorSystem);
        }

        private readonly ConcurrentDictionary<string, IActorRef> _shardGroups = new ConcurrentDictionary<string, IActorRef>();

        private readonly ClusterSharding _clusterSharding;

        private readonly IMessageExtractor _messageExtractor = new ShardedMessageMetadataExtractor();
       
        public void Execute<T>(IAggregateCommand<T> command, IMessageMetadata metadata = null, CommandConfirmationMode confirm = CommandConfirmationMode.Projected, int maxShards = 10) where T : class, IAggregate

        {
            var shardGroup = _shardGroups.GetOrAdd(typeof(T).Name,
                                                   n => _clusterSharding.Start(typeof(T).Name,
                                                                               Props.Create<AggregateActorCell<T>>(),
                                                                               ClusterShardingSettings.Create(_actorSystem),
                                                                               _messageExtractor));
            _clusterSharding.ShardRegion("");
            
            shardGroup.Tell(new ShardedCommandMetadataEnvelop(command,maxShards,metadata));
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            throw new NotImplementedException();
        }
    }

    public interface IAggregateCommand<T> : ICommand, IForAggregate<T> { }
}