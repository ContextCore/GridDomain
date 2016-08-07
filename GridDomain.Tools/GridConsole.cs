using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools
{
    /// <summary>
    /// GridConsole is used to manually issue commands to an existing grid node. 
    /// It can be used for manual domain state fixes, bebugging, support. 
    /// </summary>
    class GridConsole : IGridDomainNode, IDisposable
    {
        private readonly IAkkaNetworkAddress _server;
        private readonly ActorSystem _system;
        private IActorRef _nodeController;
        private static readonly TimeSpan NodeControllerResolveTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultCommandExecutionTimeout = TimeSpan.FromSeconds(10);
        private NodeCommandExecutor _commandExecutor;

        public GridConsole(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
        {
            _server = serverAddress;
            _system = clientConfiguration != null ? clientConfiguration.CreateInMemorySystem() : ActorSystem.Create("GridConsole");
        }

        public void Connect()
        {
            var nodeControllerSelection = 
                _system.ActorSelection($"akka.tcp://{_server.SystemName}@{_server.Host}:{_server.PortNumber}/user/{typeof(GridNodeController).Name}");

            _nodeController = nodeControllerSelection.ResolveOne(NodeControllerResolveTimeout).Result;

            _commandExecutor = new NodeCommandExecutor(_nodeController, DefaultCommandExecutionTimeout);
        }
      
        public void Dispose()
        {
            _system.Dispose();
        }

        public void Execute(params ICommand[] commands)
        {
            _commandExecutor.Execute(commands);
        }

        public Task<object> Execute(ICommand command, ExpectedMessage[] expectedMessage, TimeSpan? timeout = null)
        {
            return _commandExecutor.Execute(command, expectedMessage, timeout);
        }
    }
}
