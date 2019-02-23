using System;
using System.IO;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit.Xunit2;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Common;
using GridDomain.Node.Akka;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster.Hocon;
using GridDomain.Node.Akka.Configuration.Hocon;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Node.Tests
{
    public class DelegateActorSystemFactory : IActorSystemFactory
    {
        private readonly Func<ActorSystem> _systemCreator;

        public DelegateActorSystemFactory(Func<ActorSystem> systemCreator)
        {
            _systemCreator = systemCreator;
        }

        public ActorSystem CreateSystem()
        {
            return _systemCreator();
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
            .Build();

        private readonly IDomain _domain;


        public NodeCommandExecutionTests(ITestOutputHelper helper) : base(_config, nodeName, helper)
        {
            Serilog.Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                                          .WriteTo.File(Path.Combine("Logs", nameof(NodeCommandExecutionTests)+".log"))
                                                          .CreateLogger();
            
            var node = new GridDomainNode(new[] {new CatDomainConfiguration()},
                new DelegateActorSystemFactory(() => Sys), Serilog.Log.Logger, TimeSpan.FromSeconds(5));
            _domain = node.Start().Result;
        }

        [Fact]
        public async Task Node_can_execute_commands()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));
        }

        [Fact]
        public async Task AA_can_propagate_commands_exceptions_back()
        {
            await _domain.CommandExecutor.Execute(new Cat.GetNewCatCommand("myCat"));

            await Assert.ThrowsAsync<Cat.IsUnhappyException>(async ()=> await 
                _domain.CommandExecutor.Execute(new Cat.PetCommand("myCat")));
        }
    }
}