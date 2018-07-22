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
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Serializers;
using GridDomain.Transport;
using Serilog;

namespace GridDomain.Node {
    public abstract class GridDomainNode : IExtendedGridDomainNode
    {
        private ICommandExecutor _commandExecutor
        {
            get => _context.Executor;
            set => _context.Executor = value;
        }
        
        private IMessageWaiterFactory _waiterFactory;
        public IActorCommandPipe Pipe { get; protected set; }

        protected abstract ICommandExecutor CreateCommandExecutor();
        protected abstract IActorCommandPipe CreateCommandPipe();
        protected abstract IActorTransport CreateTransport();
        
        public IMessageWaiter NewWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewWaiter(defaultTimeout);
        }
        public Task Execute<T>(T command, IMessageMetadata metadata = null, CommandConfirmationMode mode = CommandConfirmationMode.Projected) where T : ICommand
        {
            return _commandExecutor.Execute(command, metadata, mode);
        }

        public IMessageWaiter NewExplicitWaiter(TimeSpan? defaultTimeout = null)
        {
            return _waiterFactory.NewExplicitWaiter(defaultTimeout ?? DefaultTimeout);
        }

        public ICommandExpectationBuilder Prepare<T>(T cmd, IMessageMetadata metadata = null) where T : ICommand
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
            if (_domainConfigurations.Any(d => d == null))
                throw new InvalidDomainConfigurationException();
            
            DefaultTimeout = defaultTimeout;
            Log = log;
            _actorSystemFactory = actorSystemFactory;
            EventsAdaptersCatalog = new EventsAdaptersCatalog();
            Initialize();
        }

        public EventsAdaptersCatalog EventsAdaptersCatalog { get; private set; }
        
        public IActorTransport Transport { get => _context.Transport;
            private set => _context.Transport = value;
        }

        public ActorSystem System
        {
            get => _context.System;
            private set => _context.System = value;
        }

        private IContainer Container { get; set; }
        private readonly IActorSystemFactory _actorSystemFactory;
        public ILogger Log
        {
            get => _context.Log;
            private set => _context.Log = value;
        }
        
        private readonly List<IDomainConfiguration> _domainConfigurations;
        public TimeSpan DefaultTimeout { get; }
        public Guid Id { get; } = Guid.NewGuid();

        readonly DefaultNodeContext _context = new DefaultNodeContext();
        private INodeContext Context => _context;
        
        public void Dispose()
        {
            Stop().Wait();
        }

        private void OnSystemTermination()
        {
            Log.Information("grid node Actor system terminated");
        }

        public async Task<IGridDomainNode> Start()
        {
            Log.Information("Starting GridDomain node {Id}", Id);
            
            var domainBuilder = InitDomainBuilder();

            await ConfigureDomain(domainBuilder);
            
            _commandExecutor = CreateCommandExecutor();

            BuildContainer(domainBuilder);
            
            await StartMessageRouting();

            await CreateControllerActor();

            Log.Information("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
            return this;
        }

        protected virtual async Task StartMessageRouting()
        {
            await Pipe.StartRoutes();
        }

        protected virtual async Task ConfigureDomain(DomainBuilder domainBuilder)
        {
            await domainBuilder.Configure(Pipe);
        }
        

        private async Task CreateControllerActor()
        {
            var nodeController = System.ActorOf(Props.Create(() => new GridNodeController()), nameof(GridNodeController));

            await nodeController.Ask<GridNodeController.Alive>(GridNodeController.HeartBeat.Instance);
        }

        protected void BuildContainer(DomainBuilder domainBuilder)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(new GridNodeContainerConfiguration(Context));

            System.InitDomainEventsSerialization(EventsAdaptersCatalog);

            domainBuilder.Configure(containerBuilder);

            containerBuilder.RegisterInstance(_commandExecutor);
            containerBuilder.Register(Pipe);
            
            Container = containerBuilder.Build();
            System.AddDependencyResolver(new AutoFacDependencyResolver(Container, System));
        }

        protected virtual void Initialize()
        {
            _stopping = false;

            System = _actorSystemFactory.CreateSystem();
            Pipe = CreateCommandPipe();

            System.RegisterOnTermination(OnSystemTermination);
            Transport = CreateTransport();

            _waiterFactory = CreateMessageWaiterFactory();
        }

        protected virtual IMessageWaiterFactory CreateMessageWaiterFactory()
        {
            return new LocalMessageWaiterFactory(System, Transport, DefaultTimeout);
        }

        protected abstract DomainBuilder CreateDomainBuilder();
        
        protected virtual DomainBuilder InitDomainBuilder()
        {
            var domainBuilder = CreateDomainBuilder();
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