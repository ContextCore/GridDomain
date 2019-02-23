using System;
using System.Linq;

namespace GridDomain.Node.Akka.Cluster {
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