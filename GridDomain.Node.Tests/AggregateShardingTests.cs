using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.TestKit;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Node.Tests
{

    public static class Network
    {
        public static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
    
    public class AggregateShardingTests:IDisposable
    {
        private IDomain _seedCatDomain;
        private IDomain _workerCatDomain;
        private GridDomainNode _seedNode;
        private GridDomainNode _workerNode;
     
        public const string DetailedTemplate = "{Timestamp:yy-MM-dd HH:mm:ss.fff} [{Level:u3} TH{Thread}] Src:{SourceContext}{NewLine}"
                                               + "{Message}{NewLine}"
                                               + "{Exception}";

        public AggregateShardingTests(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestOutput(output,restrictedToMinimumLevel:LogEventLevel.Verbose, outputTemplate:DetailedTemplate).CreateLogger();
            
            var nodeName = "CatWorld";
            var seedAddress = new NodeNetworkAddress("127.0.0.1",Network.FreeTcpPort(),"127.0.0.1",nodeName);
            var seed = new ActorSystemConfigBuilder().Add(new ClusterActorProviderConfig())
                .Add(LogConfig.All)
                .Add(new ClusterSeedNodes(seedAddress))
                .Add(new RemoteConfig(seedAddress))
                .Add(new ClusterActorProviderConfig())
                .Add(new TestJournalConfig())
                .Add(new ClusterBehaviorConfig(TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1)))
                .Build();

            var seedSystem = ActorSystem.Create(nodeName,seed);
            var worker = new ActorSystemConfigBuilder().Add(new ClusterActorProviderConfig())
                .Add(LogConfig.All)
                .Add(new ClusterSeedNodes(seedAddress))
                .Add(new RemoteConfig(new NodeNetworkAddress("127.0.0.1",0,"127.0.0.1",nodeName)))
                .Add(new ClusterActorProviderConfig())
                .Add(new TestJournalConfig())
                .Add(new ClusterBehaviorConfig(TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1)))
                .Build();
            
            var workerSystem = ActorSystem.Create(nodeName, worker);
            
            var catDomain = new CatDomainSettings();
            
            _seedNode = GridDomainNode.New(seedSystem,catDomain);
            _workerNode = GridDomainNode.New(workerSystem, catDomain);

          
        }


        [Fact]
        public async Task Given_two_nodes_cluster_with_aggregates_When_getting_info_about_aggregates_Then_nodes_can_provide_it_regardless_aggregate_location()
        {
            _seedCatDomain = await _seedNode.Start();
            _workerCatDomain = await _workerNode.Start();
            await Task.Delay(2000); 
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
        public async Task Given_cluster_When_new_nodes_joins_Then_it_participates_in_new_aggregates_creation()
        {
            var seedCatDomain = await _seedNode.Start();
            await seedCatDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand("Bonifaciy-the-first"));

            await _workerNode.Start();

            await Task.Delay(5000); // to form the cluster
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
            if (report.NodeAddress == _seedNode.Address)
            {
                _seedNode.Dispose();
                aliveDomain = _workerCatDomain;
                aliveNode = _workerNode;
            }
            else 
            {
                _workerNode.Dispose();
                aliveDomain = _seedCatDomain;
                aliveNode = _seedNode;
            }

            await Task.Delay(5000);
            Log.Logger.Information("sending new command");
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    await aliveDomain.CommandExecutor.Execute(new Cat.FeedCommand(catName));
                    Log.Logger.Information("trying take aggregate report");
                    report = await aliveDomain.AggregatesLifetime.GetHealth<Cat>(catName);
                    break;
                }
                catch (Exception ex)
                {
                    Log.Logger.Information($"Error: {ex}");
                }
            }

         
            Assert.Equal(aliveNode.Address, report.NodeAddress);
        }

        public void Dispose()
        {
            _seedNode?.Dispose();
            _workerNode?.Dispose();
        }
    }
}