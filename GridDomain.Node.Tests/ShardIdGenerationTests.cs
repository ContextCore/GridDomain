using System;
using GridDomain.Node.Akka.Cluster;
using Xunit;

namespace GridDomain.Node.Tests
{
    public class ShardIdGenerationTests
    {
        [Fact]
        public void ShardId_should_be_the_same_for_commands_for_the_same_aggregate_and_same_shards()
        {
            var extractor = new ShardedMessageMetadataExtractor();
            var msgA = ShardedAggregateCommand.New(new Cat.GetNewCatCommand("myCat") );
            var msgB = ShardedAggregateCommand.New(new Cat.PetCommand("myCat"));
            
            Assert.Equal(extractor.ShardId(msgA), extractor.ShardId(msgB));
        }
        
        [Fact]
        public void ShardId_resolver_should_produce_same_id_for_same_seed_and_shards_num()
        {
            IShardIdGenerator generator = new ShardIdGenerator("myShard", 25);
            
            Assert.Equal(generator.GetShardId("testSeed",100), generator.GetShardId("testSeed",100));
        }
        [Fact]
        public void ShardId_resolver_should_produce_different_id_for_same_seed_and_shards_num()
        {
            IShardIdGenerator generator = new ShardIdGenerator("myShard", 25);
            
            Assert.NotEqual(generator.GetShardId("testSeedA",100), generator.GetShardId("testSeedB",100));
        }
    }
}