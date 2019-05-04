using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Xunit;
using Akka.Cluster.TestKit;
using Akka.Cluster.Tests.MultiNode;
using Akka.Configuration;
using Akka.Remote;
using Akka.Remote.TestKit;
using Akka.TestKit;
using GridDomain.Domains;
using GridDomain.Node;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.GridDomainNodeExtension;
using GridDomain.Node.Tests;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace GridDomain.Cluster.Tests
{
    public class ShardedAggregateTests
    {
        public class AggregatesLifetimeSpec : MultiNodeClusterSpec
        {
            readonly TwoNodesClusterConfig _config;

            public AggregatesLifetimeSpec() : this(new TwoNodesClusterConfig())
            {
            }

            private AggregatesLifetimeSpec(TwoNodesClusterConfig config) : base(config,typeof(AggregatesDistributionSpec))
            {
                _config = config;
            } 
            
             [MultiNodeFact]
            public void Aggregate_lifetime_provides_information_about_aggregate_from_any_node()
            {
                IDomain seedDomain = null;
                IDomain workerDomain = null;
                
                RunOn(StartClusterNode, _config.Seed);
            
                RunOn(() => Cluster.Join(GetAddress(_config.Seed)), _config.Worker);
                
                EnterBarrier("Cluster-formed");

                RunOn(()=>
                {
                    Log.Info("from log: Starting seed grid node");

                    var seedNode = Cluster.System.InitGridDomainExtension(new CatDomainConfiguration());
                    seedDomain = seedNode.Start().Result;
                    
                    Log.Info("from log: Started seed");
                }, _config.Seed);
                
                RunOn(() =>
                {
                    Log.Info("Starting worker");
                    var workerNode = Cluster.System.InitGridDomainExtension(new CatDomainConfiguration());
                    workerDomain = workerNode.Start().Result;
                    Log.Info("Started worker");
                }, _config.Worker);

                EnterBarrier("Grid domain nodes started");
                
                RunOn(() =>
                {
                    Log.Info("Starting create aggregates from worker");
                    for (int i = 0; i < 4; i++)
                    {
                        var catName = "cat-" + i;
                        workerDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName)).Wait();
                    }  
                }, _config.Worker);
                
                RunOn(() =>
                {
                    Log.Info("Starting to get aggregate lifetime report from seed");
                    for (int i = 0; i < 4; i++)
                    {
                        var catName = "cat-" + i;
                        var report = seedDomain.AggregatesController.GetHealth<Cat>(catName).Result;
                        Assert.NotNull(report.Path);
                    }  
                }, _config.Seed);

                EnterBarrier("test finished");
            }
            
        }
        public class AggregatesDistributionSpec : MultiNodeClusterSpec
        {
            readonly TwoNodesClusterConfig _config;

            public AggregatesDistributionSpec() : this(new TwoNodesClusterConfig())
            {
            }

            private AggregatesDistributionSpec(TwoNodesClusterConfig config) : base(config,typeof(AggregatesDistributionSpec))
            {
                _config = config;
            }
            
            
            [MultiNodeFact]
            public void Created_aggregate_seed_Should_be_distributed_across_nodes()
            {
                IDomain seedDomain = null;
                IDomain workerDomain = null;
                
                RunOn(StartClusterNode, _config.Seed);
            
                RunOn(() => Cluster.Join(GetAddress(_config.Seed)), _config.Worker);
                
                EnterBarrier("Cluster-formed");

                RunOn(()=>
                {
                    Log.Info("Starting seed grid node");
                    
                    var seedNode = Cluster.System.InitGridDomainExtension(new CatDomainConfiguration());
                    seedDomain = seedNode.Start().Result;
                    Log.Info("Started seed");
                }, _config.Seed);
                
                RunOn(() =>
                {
                    Log.Info("Starting worker");
            
                    var workerNode = Cluster.System.InitGridDomainExtension(new CatDomainConfiguration());
                    workerDomain = workerNode.Start().Result;
                    
                    Log.Info("Started worker");
                }, _config.Worker);

                EnterBarrier("Grid domain nodes started");
                
                RunOn(() =>
                {
                    Log.Info("Starting issue commands from worker");
                    var knownAddresses = new List<string>();
                    for (int i = 0; i < 4; i++)
                    {
                        var catName = "cat-" + i;
                        workerDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName)).Wait();
                        var report = workerDomain.AggregatesController.GetHealth<Cat>(catName).Result;
                        Log.Info("Got report from " + report.NodeAddress);
                        knownAddresses.Add(report.NodeAddress);
                    }  

                    Assert.Equal(2, knownAddresses.Distinct().Count());
                }, _config.Worker);
                
                RunOn(() =>
                {
                    Log.Info("Starting issue commands from seed");
                    var knownAddresses = new List<string>();
                    for (int i = 0; i < 4; i++)
                    {
                        var catName = "cat-" + i;
                        workerDomain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName)).Wait();
                        var report = seedDomain.AggregatesController.GetHealth<Cat>(catName).Result;
                        Log.Info("Got report from " + report.NodeAddress);
                        knownAddresses.Add(report.NodeAddress);
                    }  
                
                    Assert.Equal(2, knownAddresses.Distinct().Count());
                }, _config.Seed);

                EnterBarrier("worker commands executed");
            }
            
             
        }
    }
}