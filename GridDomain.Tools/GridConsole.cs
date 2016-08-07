using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools
{
    class ConsoleDbConfig : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString { get; }
        public string JournalConnectionString { get; }
        public string MetadataTableName { get; }
        public string JournalTableName { get; }
        public string SnapshotTableName { get; }
    }

    class ConsoleAkkaConfiguretion : AkkaConfiguration
    {
        public ConsoleAkkaConfiguretion() : 
            base(new AkkaNetworkAddress("GridConsole","127.0.0.1",8090),
                 new ConsoleDbConfig())
        {
        }
    }

    public class TestCommand : Command
    {
        public TestCommand()
        {
            
        }
    }
    /// <summary>
    /// GridConsole is used to manually issue commands to an existing grid node. 
    /// It can be used for manual domain state fixes, bebugging, support. 
    /// </summary>
    public class GridConsole : IGridDomainNode, IDisposable
    {
        private readonly IAkkaNetworkAddress _server;
        private readonly ActorSystem _system;
        internal IActorRef _nodeController;
        private static readonly TimeSpan NodeControllerResolveTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan DefaultCommandExecutionTimeout = TimeSpan.FromSeconds(10);
        private NodeCommandExecutor _commandExecutor;
        private readonly string _remoteSystemSelectionPath;

        public GridConsole(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
            :this(serverAddress.ToSelectionPath(), clientConfiguration)
        {
        }

        public GridConsole(string remoteSystemSelectionPath, AkkaConfiguration clientConfiguration = null)
        {
            _remoteSystemSelectionPath = remoteSystemSelectionPath;

            var conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();

            _system = conf.CreateInMemorySystem();
        }

        public void Connect()
        {
            var controllerActorPath = $"{_remoteSystemSelectionPath}{typeof(GridNodeController).Name}";

            var pathA = new []
            {
                controllerActorPath,
                @"akka.tcp://LocalSystem@localhost:8080/user/GridNodeController",
                @"akka.tcp://LocalSystem@localhost:8080/user",
                @"akka.tcp://LocalSystem@localhost:8080",
                @"akka.tcp://LocalSystem@localhost:8080/user/gridnodecontroller",
                @"akka.tcp://LocalSystem@127.0.0.1:8080/user/GridNodeController",
                @"akka.tcp://LocalSystem@127.0.0.1:8080/user/gridnodecontroller",
                @"akka.tcp://LocalSystem@127.0.0.1:8080/user",
                @"akka.tcp://LocalSystem@127.0.0.1:8080"
            };

            foreach (var path in pathA)
            {
                try
                {
                     _system.ActorSelection(path).Tell(new TestCommand());
                    Thread.Sleep(1);
                    var ctr = _system.ActorSelection(path).ResolveOne(TimeSpan.FromSeconds(5)).Result;
                    Console.WriteLine("Got actor by path " + path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cant fidn path: " + path);
                }
            }

            var nodeControllerSelection = _system.ActorSelection(controllerActorPath);

             _nodeController = nodeControllerSelection.Anchor;

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
