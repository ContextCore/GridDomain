using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Console
{
    /// <summary>
    /// GridConsole is used to manually issue commands to an existing grid node. 
    /// It can be used for manual domain state fixes, bebugging, support. 
    /// </summary>
    public class GridConsole : IGridDomainNode, IDisposable
    {
        private readonly ActorSystem _consoleSystem;
        public IActorRef NodeController;
        private static readonly TimeSpan NodeControllerResolveTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan DefaultCommandExecutionTimeout = TimeSpan.FromSeconds(10);
        private NodeCommandExecutor _commandExecutor;
        private readonly IAkkaNetworkAddress _serverAddress;

        public GridConsole(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
        {
            _serverAddress = serverAddress;

            var conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();

            _consoleSystem = conf.CreateInMemorySystem();
        }

        public IActorRef GetActor(ActorSelection selection)
        {
            return selection.ResolveOne(NodeControllerResolveTimeout).Result;
        }

        public ActorSelection GetSelection(string relativePath)
        {
            var controllerActorPath = $"{_serverAddress.ToRootSelectionPath()}/{relativePath}";

             return _consoleSystem.ActorSelection(controllerActorPath);
        }

        public void Connect()
        {
            NodeController = GetActor(GetSelection(nameof(GridNodeController)));

            _commandExecutor = new NodeCommandExecutor(NodeController);
        }
      
        public void Dispose()
        {
            _consoleSystem.Dispose();
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
