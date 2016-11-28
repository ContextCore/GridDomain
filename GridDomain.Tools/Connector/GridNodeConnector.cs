using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.Akka.Remote;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Console
{
    /// <summary>
    /// GridNodeConnector is used to connect to remote node and delegate commands execution
    /// </summary>
    public class GridNodeConnector : IGridDomainNode, IDisposable
    {
        private readonly ActorSystem _consoleSystem;
        public IActorRef EventBusForwarder;
        private static readonly TimeSpan NodeControllerResolveTimeout = TimeSpan.FromSeconds(5);
        private AkkaCommandExecutor _commandExecutor;
        private readonly IAkkaNetworkAddress _serverAddress;
        private MessageWaiterFactory _gridDomainNodeImplementation;
        

        public GridNodeConnector(IAkkaNetworkAddress serverAddress, AkkaConfiguration clientConfiguration = null)
        {
            _serverAddress = serverAddress;

            var conf = clientConfiguration ?? new ConsoleAkkaConfiguretion();

            _consoleSystem = conf.CreateInMemorySystem();
            DomainEventsJsonSerializationExtensionProvider.Provider.Apply(_consoleSystem);
        }

        public IActorRef GetActor(ActorSelection selection)
        {
            return selection.ResolveOne(NodeControllerResolveTimeout).Result;
        }

        public ActorSelection GetSelection(string relativePath)
        {
            var actorPath = $"{_serverAddress.ToRootSelectionPath()}/{relativePath}";

            return _consoleSystem.ActorSelection(actorPath);
        }

        public void Connect()
        {
            EventBusForwarder = GetActor(GetSelection(nameof(EventBusForwarder)));

            var transportBridge = new RemoteAkkaEventBusTransport(
                                                new LocalAkkaEventBusTransport(_consoleSystem),
                                                EventBusForwarder,
                                                TimeSpan.FromSeconds(5));

            _commandExecutor = new AkkaCommandExecutor(_consoleSystem, transportBridge);
            _gridDomainNodeImplementation = new MessageWaiterFactory(_commandExecutor, _consoleSystem,TimeSpan.FromSeconds(30), transportBridge);
        }
      
        public void Dispose()
        {
            _consoleSystem.Dispose();
        }

        public void Execute(params ICommand[] commands)
        {
            _commandExecutor.Execute(commands);
        }

        public Task<object> Execute(CommandPlan plan)
        {
           return _commandExecutor.Execute(plan);
        }

        public Task<T> Execute<T>(CommandPlan<T> plan)
        {
            return _commandExecutor.Execute(plan);
        }

        public void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand
        {
            _commandExecutor.Execute(command, metadata);
        }

        public IMessageWaiter<Task<IWaitResults>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _gridDomainNodeImplementation.NewWaiter(defaultTimeout);
        }

        public IMessageWaiter<IExpectedCommandExecutor> NewCommandWaiter(TimeSpan? defaultTimeout = null, bool failAnyFault = true)
        {
            return _gridDomainNodeImplementation.NewCommandWaiter(defaultTimeout, failAnyFault);
        }
    }
}
