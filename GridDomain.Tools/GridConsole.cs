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
            base(new AkkaNetworkAddress("GridConsole","localhost",8090),
                 new ConsoleDbConfig())
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

        public GridConsole(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
        {
            _server = serverAddress;

            var conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();

            _system = conf.CreateInMemorySystem();
        }

        public void Connect()
        {
            // +this.Self.Path  {
         //   +this.Self.Path  {
           //     akka://LocalSystem/user/GridNodeController}	Akka.Actor.ActorPath {Akka.Actor.ChildActorPath}

                //     akka://LocalSystem/user/GridNodeController}	Akka.Actor.ActorPath {Akka.Actor.ChildActorPath}
                //  var controllerActorPath = $"akka.tcp://{_server.SystemName}@{_server.Host}:{_server.PortNumber}/user/{typeof(GridNodeController).Name}";
                var controllerActorPath = $"akka.tcp://{_server.SystemName}@{_server.Host}:{_server.PortNumber}/user/{typeof(GridNodeController).Name}";

            var pathA = new string[]
            {
                @"akka.tcp://LocalSystem@localhost:8080/user/GridNodeController",
                @"akka.tcp://LocalSystem:8080/user/GridNodeController",
                @"akka.tcp://LocalSystem@127.0.0.1:8080/user/GridNodeController",
                @"akka://LocalSystem/user/GridNodeController",
                @"akka.tcp://LocalSystem@localhost:8080/user",
            };

            foreach (var path in pathA)
            {
                try
                {
                    var ctr = _system.ActorSelection(path).Anchor;
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
