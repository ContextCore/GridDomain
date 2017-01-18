using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Connector
{
    /// <summary>
    /// GridNodeConnector is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeConnector : IGridDomainNode, IDisposable
    {
        private readonly ActorSystem _consoleSystem;
        public IActorRef EventBusForwarder;
        private static readonly TimeSpan NodeControllerResolveTimeout = TimeSpan.FromSeconds(30);
        private ICommandExecutor _busCommandExecutor;
        private readonly IAkkaNetworkAddress _serverAddress;
        private MessageWaiterFactory _waiterFactory;
        

        public GridNodeConnector(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
        {
            _serverAddress = serverAddress;

            var conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();

            _consoleSystem = conf.CreateInMemorySystem();
            DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);
        }

        public async Task<IActorRef> GetActor(ActorSelection selection)
        {
            return await selection.ResolveOne(NodeControllerResolveTimeout);
        }

        public ActorSelection GetSelection(string relativePath)
        {
            var actorPath = $"{_serverAddress.ToRootSelectionPath()}/{relativePath}";

            return _consoleSystem.ActorSelection(actorPath);
        }

        public async Task Connect()
        {
            EventBusForwarder = await GetActor(GetSelection(nameof(EventBusForwarder)));

            var defaultTimeout = TimeSpan.FromSeconds(30);

            var transportBridge = new RemoteAkkaEventBusTransport(
                                                new LocalAkkaEventBusTransport(_consoleSystem),
                                                EventBusForwarder,
                                                defaultTimeout);

            //_busCommandExecutor = new AkkaCommmandPipeExecutor();
            _waiterFactory = new MessageWaiterFactory(_consoleSystem, transportBridge, defaultTimeout);
        }
      
        public void Dispose()
        {
            _consoleSystem.Dispose();
        }

        public void Execute(params ICommand[] commands)
        {
            _busCommandExecutor.Execute(commands);
        }

        public void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand
        {
            _busCommandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _busCommandExecutor.Prepare(cmd, metadata);
        }
    }
}
