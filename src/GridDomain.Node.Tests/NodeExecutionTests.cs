using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Logger.Serilog;
using Akka.Persistence.Query;
using Akka.Streams;
using Akka.Streams.Dsl;
using Akka.TestKit;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Domains;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using GridDomain.Node.Akka.Extensions.GridDomain;
using GridDomain.Node.Tests.TestJournals;
using GridDomain.Node.Tests.TestJournals.Hocon;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Node.Tests
{
    public class NodeExecutionTests
    {
        private const string nodeName = "Node";

        private readonly NodeNetworkAddress NodeNetworkAddress =
            new NodeNetworkAddress("127.0.0.1", Network.FreeTcpPort(), "127.0.0.1", nodeName);

        private readonly IDomain _domain;
        private readonly ActorSystem _actorSystem;


        public NodeExecutionTests(ITestOutputHelper helper)// : base(_config, nodeName, helper)
        {
            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.TestOutput(helper)
                .CreateLogger();
         
            var catDomainConfiguration = new CatDomainConfiguration {MaxInactivityPeriod = TimeSpan.FromSeconds(1)};

            var config = new ActorSystemConfigBuilder()
                .Add(LogConfig.All(typeof(SerilogLogger)))
                .Add(new ClusterSeedNodes(NodeNetworkAddress))
                .Add(new RemoteConfig(NodeNetworkAddress))
                .Add(new ClusterActorProviderConfig())
                .Add(new TestJournalConfig())
                .Add(new AggregateTaggingConfig(TestJournalConfig.JournalId))
                .Build().WithFallback(TestKitBase.FullDebugConfig);
            
            _actorSystem = ActorSystem.Create(nodeName,config);
            var node = _actorSystem.InitGridDomainExtension(catDomainConfiguration);
            var clusterReady = new TaskCompletionSource<bool>();
            _domain = node.Start().Result;

            Cluster.Get(_actorSystem).RegisterOnMemberUp(() => clusterReady.SetResult(true));
            clusterReady.Task.Wait(TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Node_can_execute_commands()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));
        }
        
        [Fact]
        public void Node_can_provide_custom_command_executor()
        {
            Assert.NotNull(_domain.CommandHandler<CatCommandsHandler>());
        }
        
        [Fact]
        public async Task Node_custom_command_executor_works()
        {
            var handler = _domain.CommandHandler<CatCommandsHandler>();
            var name = await handler.Execute(new Cat.GetNewCatCommand("myCat"));
            Assert.Equal("myCat",name);
        }
        
        [Fact]
        public async Task Node_can_execute_commands_and_persist_events()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));

            var query = PersistenceQuery.Get(_actorSystem);
            var journal = query.ReadJournalFor<TestReadJournal>(TestReadJournal.Identifier);
            
            
            var source = journal.EventsByTag("Cat",NoOffset.Instance).Select(evt=>
            {
                return evt.Event as IDomainEvent;
            });
            var sink = Sink.First<IDomainEvent>();
            // connect the Source to the Sink, obtaining a RunnableGraph
            var runnable = source.ToMaterialized(sink, Keep.Right);
            
            using (var materializer = _actorSystem.Materializer())
            {
                var domainEvent = await runnable.Run(materializer);
                Assert.Equal("Cat",domainEvent.Source.Name);
                Assert.Equal("myCat",domainEvent.Source.Id);
            }
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