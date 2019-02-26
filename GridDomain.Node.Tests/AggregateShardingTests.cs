using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Xunit;

namespace GridDomain.Node.Tests
{
    public class AggregateShardingTests
    {
        [Fact(Skip="not ready yet")]
        public async Task Given_two_nodes_cluster_When_creating_aggregates_from_seed_Then_they_are_distributed_across_nodes()
        {
            var nodeName = "CatWorld";
            var seedAddress = new NodeNetworkAddress("127.0.0.1",9001,"127.0.0.1",nodeName);
            var seed = new ActorSystemConfigBuilder().Add(new ClusterActorProviderConfig())
                .Add(LogConfig.All)
                .Add(new ClusterSeedNodes(seedAddress))
                .Add(new RemoteConfig(seedAddress))
                .Add(new ClusterActorProviderConfig())
                .Add(new TestJournalConfig())
                .Build();

            var seedSystem = ActorSystem.Create(nodeName,seed);
            var worker = new ActorSystemConfigBuilder().Add(new ClusterActorProviderConfig())
                .Add(LogConfig.All)
                .Add(new ClusterSeedNodes(seedAddress))
                .Add(new RemoteConfig(new NodeNetworkAddress("127.0.0.1",0,"127.0.0.1",nodeName)))
                .Add(new ClusterActorProviderConfig())
                .Add(new TestJournalConfig())
                .Build();
            
            var workerSystem = ActorSystem.Create(nodeName, worker);
            
            var catDomain = new CatDomainConfiguration();
            
            var seedNode = GridDomainNode.New(seedSystem,catDomain);
            var workerNode = GridDomainNode.New(workerSystem, catDomain);

            var seedCatDomain = await seedNode.Start();
            var workerCatDomain = await workerNode.Start();

            var catName = "Bonifaciy";
            await seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));

            var anotherCat = "Michael";
            await seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(anotherCat));
            
           //var aggregateAPath = seedCatDomain.AggregatesLifetime.GetHealth()
            
            
        }

        [Fact]
        public async Task Given_two_nodes_cluster_with_aggregates_When_getting_info_about_aggregates_Then_nodes_can_provide_it_regardless_aggregate_location()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public async Task Given_two_nodes_cluster_When_new_nodes_join_shard_id_generator_adjusts_to_it()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public async Task Given_two_nodes_cluster_When_node_leaves_shard_id_generator_adjusts_to_it()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public async Task Given_two_nodes_cluster_When_creating_aggregates_from_worker_Then_they_are_distributed_across_nodes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Given_cluster_When_new_nodes_joins_Then_it_participates_in_new_aggregates_creation()
        {
            throw new NotImplementedException();
        }

        
        [Fact]
        public async Task Given_cluster_When_node_with_aggregate_leaves_Then_it_can_be_recreated_on_another_node()
        {
            throw new NotImplementedException();
        }
        
        [Fact]
        public async Task Given_loaded_cluster_When_new_node_joins_Then_some_aggregates_are_rebalanced()
        {
            throw new NotImplementedException();
        }
    }
}