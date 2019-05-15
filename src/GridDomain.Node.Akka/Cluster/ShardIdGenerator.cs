using System;
using System.Linq;

namespace GridDomain.Node.Akka.Cluster {
    public class ShardIdGenerator : IShardIdGenerator
    {
        private readonly string _shardGroupName;
        private readonly int _maxShards;

        public ShardIdGenerator(string shardGroupName,int maxShards)
        {
            _maxShards = maxShards;
            _shardGroupName = shardGroupName;
        }
        
        public static IShardIdGenerator Instance = new ShardIdGenerator("shard", 25);
        public string GetShardId(string seed, int? maxShards=null)
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
}