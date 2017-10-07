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
        private readonly GridNodeClient _client;

        class ServerLauncher : MarshalByRefObject
        {
            public ServerLauncher()
            {
                var nodeConfiguration = new TestGridAkkaConfiguration(5011);
               
                var node = new GridDomainNode(new []{ new BalloonDomainConfiguration() },
                                            new DelegateActorSystemFactory(() => nodeConfiguration.CreateInMemorySystem()));
                node.Start().Wait();
            }
        }

        private readonly Isolated<ServerLauncher> node;
        public GridNodeClient_Tests(ITestOutputHelper helper)
        {
            Log.Logger = new XUnitAutoTestLoggerConfiguration(helper).CreateLogger();
            _client = new GridNodeClient(new TestGridAkkaConfiguration(5011).Network);
           node = new Isolated<ServerLauncher>();
        }

        public void Dispose()
        {
            _client?.Dispose();
            node.Dispose();
        }

        [Fact]
        public async Task Console_can_wait_for_command_produced_events()
        {
            //Console_commands_are_executed_by_remote_node()
            await _client.Connect();
            var command = new InflateNewBallonCommand(42, Guid.NewGuid());
            var evt = await _client.Prepare(command)
                                   .Expect<BalloonCreated>()
                                   .Execute();

            Assert.Equal(command.Title.ToString(),
                         evt.Message<BalloonCreated>()
                            .Value);
        }

        [Fact]
        public async Task Console_can_execute_commands()
        {
            await _client.Connect();
            await _client.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()));
        }

        [Fact]
        public async Task Throws_exception_on_action_and_not_connected()
        {
            await _client.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()))
                         .ShouldThrow<NotConnectedException>();
        }

        [Fact]
        public async Task Client_can_connect()
        {
            await _client.Connect();
            Assert.True(_client.IsConnected);
        }
    }
}