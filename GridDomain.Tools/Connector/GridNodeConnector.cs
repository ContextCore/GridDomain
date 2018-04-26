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
using Serilog.Events;

namespace GridDomain.Tools.Connector {
    /// <summary>
    ///     GridNodeClient is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeConnector : IGridDomainNode
    {
        private readonly NodeConfiguration _conf;

        private readonly TimeSpan _defaultTimeout;
        private readonly NodeConfiguration _serverAddress;

        private ICommandExecutor _commandExecutor;
        private ActorSystem _consoleSystem;
        private LocalMessageWaiterFactory _waiterFactory;
        private readonly ILogger _logger;

        public GridNodeConnector New(string nodeName, string host, int port)
        {
            return new GridNodeConnector(new NodeConfiguration(nodeName,new NodeNetworkAddress(host,port)));
        }

        public GridNodeConnector(NodeConfiguration serverConfig,
                                 NodeConfiguration clientConfiguration = null,
                                 TimeSpan? defaultTimeout = null,
                                 ILogger log = null)
        {
            _logger = log ?? Log.Logger;
            _serverAddress = serverConfig;
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(60);
            _conf = clientConfiguration ?? new NodeConfiguration("Connector",new NodeNetworkAddress(), LogEventLevel.Warning);

        }

        public void Dispose()
        {
            _consoleSystem?.Dispose();
        }

        public async Task Execute<T>(T command, IMessageMetadata metadata = null, CommandConfirmationMode mode = CommandConfirmationMode.Projected) where T : ICommand
        {
            if (!IsConnected) throw new NotConnectedException();
            await _commandExecutor.Execute(command, metadata, mode);
        }


        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            if (!IsConnected)throw new NotConnectedException();
            return _waiterFactory.NewExplicitWaiter(defaultTimeout);
        }

        public ICommandExpectationBuilder Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
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
        public async Task Connect(int maxRetries = 5, TimeSpan? timeout=null)
        {
            if (_consoleSystem != null)
                return;


            IActorRef eventBusForwarder = null;
            IActorRef commandExecutionActor = null;
            int connectionCountLeft = maxRetries;
            while(true)
                try
                {
                    _consoleSystem = _conf.CreateInMemorySystem();
                    DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);
                    _logger.Information("Starting association");

                    var data = await GetSelection(nameof(GridNodeController))
                                    .Ask<GridNodeController.Connected>(GridNodeController.Connect.Instance, timeout ?? _defaultTimeout);

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

            var akkaCommandExecutor = new AkkaCommandExecutor(_consoleSystem,
                                                              transportBridge,
                                                           
                                                              _defaultTimeout);
            akkaCommandExecutor.Init(commandExecutionActor);
            
            _commandExecutor = akkaCommandExecutor;
            

            _waiterFactory = new LocalMessageWaiterFactory(_consoleSystem, transportBridge, _defaultTimeout);
        }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
    }
}