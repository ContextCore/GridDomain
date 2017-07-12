using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.Node;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Serializers;

namespace GridDomain.Tools.Connector
{
    /// <summary>
    ///     GridNodeConnector is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeConnector : IGridDomainNode
    {
        private readonly AkkaConfiguration _conf;

        private readonly TimeSpan _defaultTimeout;
        private readonly IAkkaNetworkAddress _serverAddress;

        private ICommandExecutor _commandExecutor;
        private ActorSystem _consoleSystem;
        private MessageWaiterFactory _waiterFactory;

        public GridNodeConnector(IAkkaNetworkAddress serverAddress,
                                 AkkaConfiguration clientConfiguration = null,
                                 TimeSpan? defaultTimeout = null)
        {
            _serverAddress = serverAddress;
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(60);
            _conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();
        }

        public void Dispose()
        {
            _consoleSystem.Dispose();
        }

        public Task Execute(ICommand command, IMessageMetadata metadata = null)
        {
            return _commandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewExplicitWaiter(defaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _commandExecutor.Prepare(cmd, metadata);
        }

        private async Task<IActorRef> GetActor(ActorSelection selection)
        {
            return await selection.ResolveOne(_defaultTimeout);
        }

        private ActorSelection GetSelection(string relativePath)
        {
            var actorPath = $"{_serverAddress.ToRootSelectionPath()}/{relativePath}";

            return _consoleSystem.ActorSelection(actorPath);
        }

        public async Task Connect()
        {
            if (_consoleSystem != null)
                return;

            _consoleSystem = _conf.CreateInMemorySystem();
            DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);

            var eventBusForwarder = await GetActor(GetSelection(nameof(ActorTransportProxy)));

            var transportBridge = new RemoteAkkaEventBusTransport(new LocalAkkaEventBusTransport(_consoleSystem),
                                                                  eventBusForwarder,
                                                                  _defaultTimeout);

            var commandExecutionActor = await GetActor(GetSelection(nameof(AggregatesPipeActor)));
            _commandExecutor = new AkkaCommandPipeExecutor(_consoleSystem,
                                                           transportBridge,
                                                           commandExecutionActor,
                                                           _defaultTimeout);
            _waiterFactory = new MessageWaiterFactory(_consoleSystem, transportBridge, _defaultTimeout);
        }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
    }
}