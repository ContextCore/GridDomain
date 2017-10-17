using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Connector;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.GridConsole
{

    [Collection("Grid node client collection")]
    public class GridNodeClient_Tests : IDisposable
    {

        class ServerConfiguration : NodeConfiguration
        {
            public ServerConfiguration():base("Server", new NodeNetworkAddress("localhost", 10012))
            {
                
            }
        }

        class ServerLauncher : MarshalByRefObject, IDisposable
        {
            private readonly GridDomainNode _gridDomainNode;

            public ServerLauncher()
            {
                _gridDomainNode = new GridDomainNode(new DelegateActorSystemFactory(() => new ServerConfiguration().CreateInMemorySystem()),
                    new BalloonDomainConfiguration());
                _gridDomainNode.Start().Wait();
            }

            public void Dispose()
            {
                _gridDomainNode?.Dispose();
            }
        }

        private readonly Isolated<ServerLauncher> _node;

        public GridNodeClient_Tests(ITestOutputHelper helper)
        {
            Serilog.Log.Logger = new XUnitAutoTestLoggerConfiguration(helper).CreateLogger();
           _node = new Isolated<ServerLauncher>();
        }

        public void Dispose()
        {
            _node.Dispose();
        }

        class ClientLaunch_wait_for_command_produced_events : MarshalByRefObject
        {
            public ClientLaunch_wait_for_command_produced_events()
            {
                var connector = new GridNodeConnector(new ServerConfiguration(),
                                                       new NodeConfiguration("Console",new NodeNetworkAddress()));

                connector.Connect().Wait();
                var command = new InflateNewBallonCommand(42, Guid.NewGuid());
                connector.Prepare(command)
                         .Expect<BalloonCreated>()
                         .Execute().Wait();

                Success = true;
            }

            public bool Success { get; private set; }
        }


        [Fact]
        public void Console_can_wait_for_command_produced_events()
        {
            var isolatedClient = new Isolated<ClientLaunch_wait_for_command_produced_events>();
            Assert.True(isolatedClient.Value.Success);
            isolatedClient.Dispose();
        }

        class Isolated_Console_can_execute_commands : MarshalByRefObject
        {
            public Isolated_Console_can_execute_commands()
            {
                var connector = new GridNodeConnector(new ServerConfiguration(),
                    new NodeConfiguration("Console", new NodeNetworkAddress()));

                connector.Connect().Wait();
                var command = new InflateNewBallonCommand(42, Guid.NewGuid());
                connector.Execute(command).Wait();

                Success = true;
            }

            public bool Success { get; private set; }
        }


        [Fact]
        public void Console_can_execute_commands()
        {
            var isolatedClient = new Isolated<Isolated_Console_can_execute_commands>();
            Assert.True(isolatedClient.Value.Success);
            isolatedClient.Dispose();
        }

        [Fact]
        public async Task Throws_exception_on_action_and_not_connected()
        {
            var connector = new GridNodeConnector(new ServerConfiguration(), new NodeConfiguration("Console", new NodeNetworkAddress()));
            await connector.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()))
                            .ShouldThrow<NotConnectedException>();
        }


        class Isolated_Client_can_connect : MarshalByRefObject
        {
            public Isolated_Client_can_connect()
            {
                var connector = new GridNodeConnector(new ServerConfiguration(),
                    new NodeConfiguration("Console", new NodeNetworkAddress()));

                connector.Connect().Wait();
                Success = true;
            }

            public bool Success { get; }
        }
        [Fact]
        public void Client_can_connect()
        {
            var isolatedClient = new Isolated<Isolated_Client_can_connect>();
            Assert.True(isolatedClient.Value.Success);
            isolatedClient.Dispose();
        }
    }
}