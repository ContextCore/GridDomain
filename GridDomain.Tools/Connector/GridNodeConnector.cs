using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Configuration;
using GridDomain.Node.Serializers;
using GridDomain.Transport;
using GridDomain.Transport.Remote;
using Serilog;

namespace GridDomain.Tools.Connector
{

    public class NotConnectedException : Exception
    {
    }
    /// <summary>
    ///     GridNodeClient is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeClient : IGridDomainNode
    {
        private readonly NodeConfiguration _conf;

        private readonly TimeSpan _defaultTimeout;
        private readonly INodeNetworkAddress _serverAddress;

        private ICommandExecutor _commandExecutor;
        private ActorSystem _consoleSystem;
        private MessageWaiterFactory _waiterFactory;
        private readonly bool _retryConnect;
        private readonly ILogger _logger;

        public GridNodeClient(INodeNetworkAddress serverAddress,
                              NodeConfiguration clientConfiguration = null,
                              TimeSpan? defaultTimeout = null,
                              bool retryConnect = true,
                              ILogger log = null)
        {
            _logger = log;
            _retryConnect = retryConnect;
            _serverAddress = serverAddress;
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(60);
            _conf = clientConfiguration ?? new ConsoleNodeConfiguration();
        }

        public void Dispose()
        {
            _consoleSystem?.Dispose();
        }

        public async Task Execute(ICommand command, IMessageMetadata metadata = null)
        {
            if (!IsConnected) throw new NotConnectedException();
            await _commandExecutor.Execute(command, metadata);
        }


        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            if (!IsConnected)throw new NotConnectedException();
            return _waiterFactory.NewExplicitWaiter(defaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            if(!IsConnected) throw new NotConnectedException();
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
        public int ConnectionRetries { get; private set; }
        public bool IsConnected => _commandExecutor != null;
        public async Task Connect(CancellationToken? token = null)
        {
            if (_consoleSystem != null)
                return;

            _consoleSystem = _conf.CreateInMemorySystem();
            DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);

            IActorRef eventBusForwarder = null;
            IActorRef commandExecutionActor = null;

            while (_retryConnect && (!token.HasValue || token?.IsCancellationRequested == false))
            {
                try
                {
                    eventBusForwarder = await GetActor(GetSelection("ActorTransportProxy"));
                    commandExecutionActor = await GetActor(GetSelection(nameof(AggregatesPipeActor)));
                    break;
                }
                catch (Exception ex)
                {
                    ConnectionRetries ++;
                    (_logger ?? Log.Logger).Error(ex,"Could not connect to griddomain node at {@adress}",_serverAddress);
                }
            }
            
            var transportBridge = new RemoteAkkaEventBusTransport(new LocalAkkaEventBusTransport(_consoleSystem),
                                                                      eventBusForwarder,
                                                                      _defaultTimeout);
                
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