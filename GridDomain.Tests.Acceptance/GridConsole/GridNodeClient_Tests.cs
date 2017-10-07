using System;
using System.Threading.Tasks;
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
        private readonly GridNodeConnector _connector;

        public GridNodeClient_Tests(ITestOutputHelper helper)
        {
            Log.Logger = new XUnitAutoTestLoggerConfiguration(helper).CreateLogger();
            _connector = new GridNodeConnector(new ServerConfiguration(),new NodeConfiguration("Console",new NodeNetworkAddress()));
           _node = new Isolated<ServerLauncher>();
        }

        public void Dispose()
        {
            _connector?.Dispose();
            _node.Dispose();
        }

        [Fact]
        public async Task Console_can_wait_for_command_produced_events()
        {
            //Console_commands_are_executed_by_remote_node()
            await _connector.Connect();
            var command = new InflateNewBallonCommand(42, Guid.NewGuid());
            var evt = await _connector.Prepare(command)
                                   .Expect<BalloonCreated>()
                                   .Execute();

            Assert.Equal(command.Title.ToString(),
                         evt.Message<BalloonCreated>()
                            .Value);
        }

        [Fact]
        public async Task Console_can_execute_commands()
        {
            await _connector.Connect();
            await _connector.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()));
        }

        [Fact]
        public async Task Throws_exception_on_action_and_not_connected()
        {
            await _connector.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()))
                         .ShouldThrow<NotConnectedException>();
        }

        [Fact]
        public async Task Client_can_connect()
        {
            await _connector.Connect();
            Assert.True(_connector.IsConnected);
        }
    }
}