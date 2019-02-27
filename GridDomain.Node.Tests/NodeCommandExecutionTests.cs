using System;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Persistence;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Actors;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Tests.TestJournals;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Node.Tests
{

    public class CatAggregateSerializationTests
    {
        [Fact]
        public void Commands_are_serializable()
        {
            var cmd = new Cat.GetNewCatCommand("mau");
            var jsonString = JsonConvert.SerializeObject(cmd);
            var restoredCmd = JsonConvert.DeserializeObject(jsonString);
        }
        
        [Fact]
        public void ShardedCommands_are_serializable()
        {
            var cmd = ShardedAggregateCommand.New(new Cat.GetNewCatCommand("mau"));
            var jsonString = JsonConvert.SerializeObject(cmd);
            var restoredCmd = JsonConvert.DeserializeObject(jsonString);
        }
    }
    
    public class NodeCommandExecutionTests : TestKit
    {
        private const string nodeName = "Node";

        private static readonly NodeNetworkAddress NodeNetworkAddress =
            new NodeNetworkAddress("127.0.0.1", 9001, "127.0.0.1", nodeName);

        private static readonly Config _config = new ActorSystemConfigBuilder()
            .Add(LogConfig.All)
            .Add(new ClusterSeedNodes(NodeNetworkAddress))
            .Add(new RemoteConfig(NodeNetworkAddress))
            .Add(new ClusterActorProviderConfig())
            .Add(new TestJournalConfig())
            .Build();

        private readonly IDomain _domain;
        private readonly CatDomainConfiguration _catDomainConfiguration;


        public NodeCommandExecutionTests(ITestOutputHelper helper) : base(_config, nodeName, helper)
        {
            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(helper)
                                                          //.WriteTo.File(Path.Combine("Logs", nameof(NodeCommandExecutionTests)+".log"),LogEventLevel.Verbose)
                                                          .CreateLogger();


            _catDomainConfiguration = new CatDomainConfiguration {MaxInactivityPeriod = TimeSpan.FromSeconds(1)};

            var node = new GridDomainNode(new[] {_catDomainConfiguration},
                new DelegateActorSystemFactory(() => Sys), Serilog.Log.Logger, TimeSpan.FromSeconds(5));
            _domain = node.Start().Result;
        }

        [Fact]
        public async Task Node_can_execute_commands()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));
        }
        
        [Fact]
        public async Task Node_can_execute_commands_and_persist_events()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));

            var query = PersistenceQuery.Get(Sys);
            var journal = query.ReadJournalFor<TestReadJournal>(TestReadJournal.Identifier);
            
            
            var source = journal.CurrentPersistenceIds().Select(id=>AggregateAddress.Parse<Cat>(id).Id);
            var sink = Sink.First<string>();
            // connect the Source to the Sink, obtaining a RunnableGraph
            var runnable = source.ToMaterialized(sink, Keep.Right);
            
            using (var materializer = Sys.Materializer())
            {
                var persistedId = await runnable.Run(materializer);
                Assert.Equal("myCat",persistedId);
            }
        }
        
        [Fact]
        public async Task Node_can_restore_aggregate_from_persistence()
        {
            var catName = "myCat";
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
            await _domain.CommandExecutor.Execute(new Cat.FeedCommand(catName));
          
            var report = await _domain.AggregatesLifetime.GetHealth(AggregateAddress.New<Cat>(catName));
            var actor = await Sys.ActorSelection(report.Path).ResolveOne(TimeSpan.FromSeconds(2));
            Watch(actor);
            ExpectTerminated(actor);
            
            //no exception - means we've pet the cat already fed
            await _domain.CommandExecutor.Execute(new Cat.PetCommand(catName));
        }

        [Fact]
        public async Task Node_will_shutdown_an_aggregate_on_inactivity()
        {
            var catName = "myCat";
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
            var report = await _domain.AggregatesLifetime.GetHealth(catName.AsAddressFor<Cat>());
            var actor = await Sys.ActorSelection(report.Path).ResolveOne(TimeSpan.FromSeconds(2));
            Watch(actor);
            ExpectTerminated(actor);
                
        }
        
        [Fact]
        public async Task AggregateLifetime_can_locate_aggregates()
        {
            var catName = "myCat";
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand(catName));
            Assert.NotNull(await _domain.AggregatesLifetime.GetHealth(typeof(Cat).AsAddress(catName)));
        }
        
        [Fact]
        public async Task Node_can_propagate_commands_exceptions_back()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));

            await Assert.ThrowsAsync<Cat.IsUnhappyException>(async ()=> await 
                _domain.CommandExecutor.Execute(new Cat.PetCommand("myCat")));
        }
    }
}