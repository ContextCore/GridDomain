using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.TestKit;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Node.Tests
{
    public class AggregateShardingTests
    {
        private IDomain _seedCatDomain;
        private IDomain _workerCatDomain;
        private GridDomainNode _seedNode;
        private GridDomainNode _workerNode;

        public AggregateShardingTests(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(output).CreateLogger();
            
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
            
            _seedNode = GridDomainNode.New(seedSystem,catDomain);
            _workerNode = GridDomainNode.New(workerSystem, catDomain);

          
        }
        
        [Fact]
        public async Task Given_two_nodes_cluster_When_creating_aggregates_from_seed_Then_they_are_distributed_across_nodes()
        {
            _seedCatDomain = await _seedNode.Start();
            _workerCatDomain = await _workerNode.Start();

            var knownAddresses = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                var catName = "cat-" + i;
                await _seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
                var report = await _workerCatDomain.AggregatesLifetime.GetHealth<Cat>(catName);
                Log.Logger.Information(report.NodeAddress);
                knownAddresses.Add(report.NodeAddress);
            }  

            Assert.Equal(2, knownAddresses.Distinct().Count());
        }

        [Fact]
        public async Task Given_two_nodes_cluster_with_aggregates_When_getting_info_about_aggregates_Then_nodes_can_provide_it_regardless_aggregate_location()
        {
            _seedCatDomain = await _seedNode.Start();
            _workerCatDomain = await _workerNode.Start();

            for (int i = 0; i < 4; i++)
            {
                var catName = "Bonifaciy-" + i;
                await _workerCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
                var report = await _seedCatDomain.AggregatesLifetime.GetHealth<Cat>(catName);
                Log.Logger.Information(report.NodeAddress);
                Assert.NotNull(report.Path);
            }  
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
        public async Task Given_cluster_When_new_nodes_joins_Then_it_participates_in_new_aggregates_creation()
        {
            var seedCatDomain = await _seedNode.Start();
            await seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand("Bonifaciy-the-first"));

            await _workerNode.Start();

            await Task.Delay(2000); // to form the cluster
            var addresses = new List<string>();
            for (int i = 0; i < 4; i++)
            {
                var catName = "Bonifaciy-the-"+i;
                await seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
                var report = await seedCatDomain.AggregatesLifetime.GetHealth<Cat>(catName);
                addresses.Add(report.NodeAddress);
            }
            Assert.Contains(addresses,s => s == _workerNode.Address);
        }

        
        [Fact]
        public async Task Given_cluster_When_node_with_aggregate_leaves_Then_it_can_be_recreated_on_another_node()
        {
            _seedCatDomain = await _seedNode.Start();
            _workerCatDomain = await _workerNode.Start();
                
            var catName = "Bonifaciy-the-first";
            await _workerCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
            var report = await _workerCatDomain.AggregatesLifetime.GetHealth<Cat>(catName);

            IDomain aliveDomain;
            INode aliveNode;
           // TaskCompletionSource<int> nodeLeft = new TaskCompletionSource<int>();
            if (report.NodeAddress == _seedNode.Address)
            {
                _seedNode.Dispose();
                aliveDomain = _workerCatDomain;
                aliveNode = _workerNode;
               // var cluster = Cluster.Get(_workerNode.System);
               // cluster.(() => nodeLeft.SetResult(0));
            }
            else 
            {
                _workerNode.Dispose();
                aliveDomain = _seedCatDomain;
                aliveNode = _seedNode;
              ///  var cluster = Cluster.Get(_seedNode.System);
               // cluster.RegisterOnMemberRemoved(() => nodeLeft.SetResult(0));
            }

            await Task.Delay(10000);//nodeLeft.Task);
            Log.Logger.Information("sending new command");
            await aliveDomain.CommandExecutor.Execute(new Cat.FeedCommand(catName));
            Log.Logger.Information("trying take aggregate report");
            report = await aliveDomain.AggregatesLifetime.GetHealth<Cat>(catName);
            Assert.Equal(aliveNode.Address, report.NodeAddress);
        }
        
        [Fact]
        public async Task Given_loaded_cluster_When_new_node_joins_Then_some_aggregates_are_rebalanced()
        {
            throw new NotImplementedException();
        }
    }
}