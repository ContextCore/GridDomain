using System;
using GridDomain.Node.Cluster;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    public class ShardIdGenerationTests
    {
        private ClusterInfo _akkaCluster;
        private readonly Logger _logger;
        private readonly ITestOutputHelper _testOutputHelper;

        public ShardIdGenerationTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }

        [Fact]
        public void TestRandom_from_seed()
        {
            var random = new Random(123);
            Assert.Equal(9,random.Next(10));
            Assert.Equal(9,random.Next(10));
            Assert.Equal(7,random.Next(10));
        }

        [Fact]
        public void ShardId_should_be_the_same_for_commands_for_the_same_aggregate_and_same_shards()
        {
            var extractor = new ShardedMessageMetadataExtractor();
            var msgA = new ShardedCommandMetadataEnvelop(new BlowBalloonCommand("myAggregate"));
            var msgB = new ShardedCommandMetadataEnvelop(new WriteTitleCommand(12,"myAggregate"));
            
            Assert.Equal(extractor.ShardId(msgA), extractor.ShardId(msgB));
        }
        
        [Fact]
        public void ShardId_resolver_should_produce_same_id_for_same_seed_and_shards_num()
        {
            IShardIdGenerator generator = new DefaultShardIdGenerator("myShard");
            
            Assert.Equal(generator.Resolve("testSeed",100), generator.Resolve("testSeed",100));
        }
        [Fact]
        public void ShardId_resolver_should_produce_different_id_for_same_seed_and_shards_num()
        {
            IShardIdGenerator generator = new DefaultShardIdGenerator("myShard");
            
            Assert.NotEqual(generator.Resolve("testSeedA",100), generator.Resolve("testSeedB",100));
        }
    }
}