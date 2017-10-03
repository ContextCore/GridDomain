using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Connector;
using Xunit;

namespace GridDomain.Tests.Unit.GridConsole
{

    [Collection("Grid node client collection")]
    public class GridNodeClient_Tests : IDisposable
    {
        private readonly GridNodeClient _client;
        private readonly GridDomainNode _node;

        public GridNodeClient_Tests()
        {
            var nodeConfiguration = new TestGridNodeConfiguration(5010);
            var nodeAddress = nodeConfiguration.Network;
            var settings = new NodeSettings(() => nodeConfiguration.CreateInMemorySystem());
            settings.Add(new BalloonDomainConfiguration());
            _node = new GridDomainNode(settings);
            _node.Start()
                 .Wait();
            _client = new GridNodeClient(nodeAddress);
        }

        public void Dispose()
        {
            _node?.Dispose();
            _client?.Dispose();
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