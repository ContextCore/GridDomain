using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Node.Serializers;
using GridDomain.Transport;
using GridDomain.Transport.Remote;
using Serilog;

namespace GridDomain.Tools.Connector {
    /// <summary>
    ///     GridNodeClient is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeClient : IGridDomainNode
    {
        private readonly AkkaConfiguration _conf;

        private readonly TimeSpan _defaultTimeout;
        private readonly INodeNetworkAddress _serverAddress;

        private ICommandExecutor _commandExecutor;
        private ActorSystem _consoleSystem;
        private MessageWaiterFactory _waiterFactory;
        private readonly ILogger _logger;

        public GridNodeClient(INodeNetworkAddress serverAddress,
                              AkkaConfiguration clientConfiguration = null,
                              TimeSpan? defaultTimeout = null,
                              ILogger log = null)
        {
            _logger = log ?? Log.Logger;
            _serverAddress = serverAddress;
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(60);
            _conf = clientConfiguration ?? new ConsoleAkkaConfiguration();

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

        private ActorSelection GetSelection(string relativePath)
        {
            var actorPath = $"{_serverAddress.ToRootSelectionPath()}/{relativePath}";

            return _consoleSystem.ActorSelection(actorPath);
        }

        public bool IsConnected => _commandExecutor != null;
        public async Task Connect()
        {
            if (_consoleSystem != null)
                return;

            _logger.Information("Sending warmup message to start association");

            IActorRef eventBusForwarder = null;
            IActorRef commandExecutionActor = null;
            int connectionCountLeft = 5;
            while(true)
                try
                {
                    _consoleSystem = _conf.CreateInMemorySystem();
                    DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);


                    var data = await GetSelection(nameof(GridNodeController)).Ask<GridNodeController.Connected>(GridNodeController.Connect.Instance, TimeSpan.FromSeconds(10));
                    eventBusForwarder = data.TransportProxy;
                    commandExecutionActor = data.PipeRef;

                    _logger.Information("Association formed");
                    break;
                }
                catch (Exception ex)
                {
                    _consoleSystem?.Dispose();
                    _logger.Warning(ex,"could not get answer from grid node controller in time");
                    if (--connectionCountLeft == 0)
                        throw;
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