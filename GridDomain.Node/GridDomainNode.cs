using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Serializers;
using GridDomain.Transport;
using GridDomain.Transport.Extension;
using Serilog;

namespace GridDomain.Node {


    public class GridDomainLocalNode : GridDomainNode
    {
        public GridDomainLocalNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        protected override ICommandExecutor CreateCommandExecutor()
        {
            var executor = new AkkaCommandExecutor(System,Transport,DefaultTimeout);
            executor.Init(Pipe.CommandExecutor);
            return executor;
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new LocalCommandPipe(System);
        }
    }
    
    public abstract class GridDomainNode : IGridDomainNode
    {
        private ICommandExecutor _commandExecutor;
        private IMessageWaiterFactory _waiterFactory;
        protected internal IActorCommandPipe Pipe;

        protected abstract ICommandExecutor CreateCommandExecutor();
        protected abstract IActorCommandPipe CreateCommandPipe();
        
        public IMessageWaiter<Task<IWaitResult>> NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
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
        
        private bool _stopping;

        protected GridDomainNode(IEnumerable<IDomainConfiguration> domainConfigurations, 
                                  IActorSystemFactory actorSystemFactory,
                                  ILogger log, 
                                  TimeSpan defaultTimeout)
        {
            _domainConfigurations = domainConfigurations.ToList();
            if(!_domainConfigurations.Any())
                throw new NoDomainConfigurationException();

            DefaultTimeout = defaultTimeout;
            Log = log;
            _actorSystemFactory = actorSystemFactory;
        }

        public EventsAdaptersCatalog EventsAdaptersCatalog { get; private set; }
        public IActorTransport Transport { get; private set; }
        public ActorSystem System { get; private set; }
        private IContainer Container { get; set; }
        private readonly IActorSystemFactory _actorSystemFactory;
        public ILogger Log { get; }
        private readonly List<IDomainConfiguration> _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public event EventHandler<GridDomainNode> Initializing = delegate { };

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
           
            Initialize();
            
            var domainBuilder = CreateDomainBuilder();

            ConfigureContainer(domainBuilder);

            await ConfigurePipe(domainBuilder);

            await CreateControllerActor();

            Log.Information("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
        }

        private async Task ConfigurePipe(DomainBuilder domainBuilder)
        {
            await domainBuilder.Configure(Pipe);
            await Pipe.BuildRoutes();
        }

        private async Task CreateControllerActor()
        {
            var nodeController = System.ActorOf(Props.Create(() => new GridNodeController()), nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Alive>(GridNodeController.HeartBeat.Instance);
        }

        private void ConfigureContainer(DomainBuilder domainBuilder)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(new GridNodeContainerConfiguration(Transport, Log));

            Initializing.Invoke(this, this);

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            domainBuilder.Configure(containerBuilder);

            _commandExecutor = CreateCommandExecutor();
            containerBuilder.RegisterInstance(_commandExecutor);
            containerBuilder.Register(Pipe);
            
            Container = containerBuilder.Build();
            System.AddDependencyResolver(new AutoFacDependencyResolver(Container, System));
        }

        private void Initialize()
        {
            _stopping = false;
            EventsAdaptersCatalog = new EventsAdaptersCatalog();

            System = _actorSystemFactory.CreateSystem();
            Pipe = CreateCommandPipe();

            System.RegisterOnTermination(OnSystemTermination);
            Transport = System.GetTransport();

            _waiterFactory = new MessageWaiterFactory(System, Transport, DefaultTimeout);
        }

        private DomainBuilder CreateDomainBuilder()
        {
            var domainBuilder = new DomainBuilder();
            _domainConfigurations.ForEach(c => domainBuilder.Register(c));
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

    }
}