using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.CommandPipe;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    
    //public class AnyMessageClusterPublisher:AnyMessagePublisher
    //{
    //    public AnyMessageClusterPublisher(IActorCommandPipe commandPipe, MessagesWaiter<AnyMessagePublisher> waiter) : base(commandPipe, waiter) { }
    //    
    //    protected override object EnvelopeProcessMessage(DomainEvent message, IMessageMetadata metadata)
    //    {
    //        return new ShardedProcessMessageMetadataEnvelop(message,message.ProcessId,metadata ?? MessageMetadata.Empty);
    //    }
    //
    //}
    
    public class ShardIdGenerationTests
    {
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
            
            Assert.Equal(generator.GetShardId("testSeed",100), generator.GetShardId("testSeed",100));
        }
        [Fact]
        public void ShardId_resolver_should_produce_different_id_for_same_seed_and_shards_num()
        {
            IShardIdGenerator generator = new DefaultShardIdGenerator("myShard");
            
            Assert.NotEqual(generator.GetShardId("testSeedA",100), generator.GetShardId("testSeedB",100));
        }
    }
}