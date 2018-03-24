using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Akka.Util.Internal;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Serializers;
using GridDomain.Transport.Remote;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Serilog;
using Serilog.Core;
using ICommand = GridDomain.CQRS.ICommand;

namespace GridDomain.Node
{

    public class GridNodeBuilder
    {
        protected IActorSystemFactory _actorSystemFactory;
        protected ILogger _logger;
        protected IDomainConfiguration[] _domainConfigurations;
        protected Func<IActorCommandPipe> _commandPipe;
        protected TimeSpan _timeout;
        
        public GridNodeBuilder()
        {
            _logger = new DefaultLoggerConfiguration().CreateLogger()
                                                      .ForContext<GridDomainNode>();
            _timeout = TimeSpan.FromSeconds(10);
            _actorSystemFactory = new HoconActorSystemFactory("system","");
            _commandPipe = new LocalCommandPipe(_actorSystemFactory.Create());
        }
        
        public IGridDomainNode Build()
        {
            return new GridDomainNode(_domainConfigurations,_actorSystemFactory,_commandPipe,_logger,_timeout);
        }

        public GridNodeBuilder ActorFactory(IActorSystemFactory factory)
        {
            _actorSystemFactory = factory;
            return this;
        }

        public GridNodeBuilder Log(ILogger log)
        {
            _logger = log;
            return this;
        }
        
        public GridNodeBuilder DomainConfigurations(params IDomainConfiguration[] domainConfigurations)
        {
            _domainConfigurations = domainConfigurations;
            return this;
        } 
        
        public GridNodeBuilder CommandPipeFactory(Func<IActorCommandPipe> commandPipe)
        {
            _commandPipe = commandPipe;
            return this;
        }

        public GridNodeBuilder Timeout(TimeSpan timeout)
        {
            this._timeout = timeout;
            return this;
        }

    }
    
    public class GridDomainNode : IGridDomainNode
    {
        private ICommandExecutor _commandExecutor;

        private bool _stopping;
        private IMessageWaiterFactory _waiterFactory;
        internal IActorCommandPipe Pipe;

        public GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations, 
                              IActorSystemFactory actorSystemFactory,
                              Func<IActorCommandPipe> commandPipeFactory,
                              ILogger log, 
                              TimeSpan defaultTimeout)
        {
            _commandPipeFactory = commandPipeFactory;
            DomainConfigurations = domainConfigurations.ToList();
            if(!DomainConfigurations.Any())
                throw new NoDomainConfigurationException();

            DefaultTimeout = defaultTimeout;
            Log = log;
            _actorSystemFactory = actorSystemFactory;
        }

        public EventsAdaptersCatalog EventsAdaptersCatalog { get; private set; }
        public IActorTransport Transport { get; private set; }
        public ActorSystem System { get; private set; }

        private IContainer Container { get; set; }
        private ContainerBuilder _containerBuilder;
        private readonly IActorSystemFactory _actorSystemFactory;
        public ILogger Log { get; }
        internal readonly List<IDomainConfiguration> DomainConfigurations;
        private IActorRef _commandExecutorActor;
        private IMessagesRouter _messageRouter;
        private readonly Func<IActorCommandPipe> _commandPipeFactory;
        public TimeSpan DefaultTimeout { get; }

        public Guid Id { get; } = Guid.NewGuid();
        public event EventHandler<GridDomainNode> Initializing = delegate { };

        public Task Execute<T>(T command, IMessageMetadata metadata = null, CommandConfirmationMode mode = CommandConfirmationMode.Projected) where T : ICommand
        {
            return _commandExecutor.Execute(command, metadata, mode);
        }

        public IMessageWaiter<Task<IWaitResult>> NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewExplicitWaiter(defaultTimeout ?? DefaultTimeout);
        }

        public ICommandWaiter Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
        {
            return _commandExecutor.Prepare(cmd, metadata);
        }

        public void Dispose()
        {
            Stop().Wait();
        }

        private void OnSystemTermination()
        {
            Log.Information("grid node Actor system terminated");
        }

        public async Task Start()
        {
            Log.Information("Starting GridDomain node {Id}", Id);

            _stopping = false;
            EventsAdaptersCatalog = new EventsAdaptersCatalog();
            _containerBuilder = new ContainerBuilder();

            System = _actorSystemFactory.Create();
            System.RegisterOnTermination(OnSystemTermination);
            Transport = System.GetTransport();

            _containerBuilder.Register(new GridNodeContainerConfiguration(Transport, Log));
            _waiterFactory = new MessageWaiterFactory(System, Transport, DefaultTimeout);

            Initializing.Invoke(this, this);

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            IActorCommandPipe pipe = _commandPipeFactory();
            
            _commandExecutorActor = await pipe.Init(_containerBuilder);
            _messageRouter = pipe;
            Pipe = pipe;
            _commandExecutor = new AkkaCommandExecutor(System, Transport, _commandExecutorActor, DefaultTimeout);
            _containerBuilder.RegisterInstance(_commandExecutor);

            var domainBuilder = CreateDomainBuilder();
            domainBuilder.Configure(_containerBuilder);

            Container = _containerBuilder.Build();
            System.AddDependencyResolver(new AutoFacDependencyResolver(Container, System));
            
            domainBuilder.Configure(_messageRouter);
            
            var nodeController = System.ActorOf(Props.Create(() => new GridNodeController(_commandExecutorActor)), nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Alive>(GridNodeController.HeartBeat.Instance);

            Log.Information("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private DomainBuilder CreateDomainBuilder()
        {
            var domainBuilder = new DomainBuilder();
          
            DomainConfigurations.ForEach(c => domainBuilder.Register(c));
            return domainBuilder;
        }

        public async Task Stop()
        {
            if(_stopping)
                return;

            Log.Information("GridDomain node {Id} is stopping", Id);
            _stopping = true;

            if(System != null)
            {
                await System.Terminate();
                System.Dispose();
            }
            System = null;
            Container?.Dispose();
            Log.Information("GridDomain node {Id} stopped", Id);
        }

        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
    }
}