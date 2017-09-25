using System;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tools.Connector;
using Xunit;

namespace GridDomain.Tests.Unit.GridConsole
{
    public class GridConsole_Command_executions_Tests : IDisposable
    {
        private GridNodeConnector _gridNodeConnector;
        private GridDomainNode _gridDomainNode;

        public void Dispose()
        {
            _gridDomainNode?.Dispose();
            _gridNodeConnector?.Dispose();
        }

        [Fact
#if DEBUG
            (Skip ="console system should not serialize creators, run test in release mode")
#endif
            ]
        public async Task Console_can_wait_for_command_produced_events()
        {
           await CreateConsole(9003);
            //Console_commands_are_executed_by_remote_node()
            var command = new InflateNewBallonCommand(42, Guid.NewGuid());

            var evt = await _gridNodeConnector.Prepare(command)
                                     .Expect<BalloonCreated>()
                                     .Execute();

            Assert.Equal(command.Title.ToString(), evt.Message<BalloonCreated>().Value);
        }

        [Fact
#if DEBUG
            (Skip = "console system should not serialize creators, run test in release mode")
#endif
        ]

        public async Task Console_can_execute_commands()
        {
            await CreateConsole(9004);

            await _gridNodeConnector.Execute(new InflateNewBallonCommand(42, Guid.NewGuid()));
        }

        [Fact
#if DEBUG
            (Skip = "console system should not serialize creators, run test in release mode")
#endif
        ]
        public async Task Console_can_connect()
        {
            await CreateConsole(9002);
            Assert.NotNull(_gridNodeConnector);
        }

        private async Task CreateConsole(int port)
        {
            var serverConfig = new TestGridNodeConfiguration(port);
            var settings = new NodeSettings(() => serverConfig.CreateInMemorySystem());
            settings.Add(new BalloonDomainConfiguration());
            _gridDomainNode = new GridDomainNode(settings);

            await _gridDomainNode.Start();
            _gridNodeConnector = new GridNodeConnector(serverConfig.Network);
            await _gridNodeConnector.Connect();
        }
    }
}