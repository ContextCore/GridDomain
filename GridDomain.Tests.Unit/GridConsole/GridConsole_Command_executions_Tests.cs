using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Connector;
using Microsoft.Practices.Unity;
using Xunit;

namespace GridDomain.Tests.Unit.GridConsole
{
    public class GridConsole_Command_executions_Tests : IDisposable
    {
        public void Dispose()
        {
            _serverNode.Dispose();
            _connector.Dispose();
        }

        private GridNodeConnector _connector;
        private GridDomainNode _serverNode;

        //[Fact]

        public async Task Given_existing_GridNode()
        {
            var serverConfig = new TestGridNodeConfiguration();

            var settings = new NodeSettings(() => serverConfig.CreateInMemorySystem());

            settings.DomainBuilder.Register(new BalloonDomainConfiguration());

            _serverNode = new GridDomainNode(settings);

            await _serverNode.Start();

            _connector = new GridNodeConnector(serverConfig.Network);

            await _connector.Connect();
            //Console_commands_are_executed_by_remote_node()
            var command = new InflateNewBallonCommand(42, Guid.NewGuid());

            var evt = await _connector.Prepare(command).Expect<BalloonCreated>().Execute();

            Assert.Equal(command.Title.ToString(), evt.Message<BalloonCreated>().Value);
        }
    }
}