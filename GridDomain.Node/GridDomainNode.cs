using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
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
        private AkkaCommandExecutor _akkaCommandExecutor;
        public GridDomainLocalNode(IEnumerable<IDomainConfiguration> domainConfigurations, IActorSystemFactory actorSystemFactory, ILogger log, TimeSpan defaultTimeout) : base(domainConfigurations, actorSystemFactory, log, defaultTimeout) { }
        
        protected override ICommandExecutor CreateCommandExecutor()
        {
            _akkaCommandExecutor = new AkkaCommandExecutor(System,Transport,DefaultTimeout);
            return _akkaCommandExecutor;
        }

        protected override IActorCommandPipe CreateCommandPipe()
        {
            return new LocalCommandPipe(System);
        }

        protected override IActorTransport CreateTransport()
        {
            return System.InitLocalTransportExtension().Transport;
        }

        protected override async Task StartMessageRouting()
        {
            await base.StartMessageRouting();
            _akkaCommandExecutor.Init(Pipe.CommandExecutor);
        }
    }

    public interface INodeContext :IMessageProcessContext
    {
           ActorSystem System { get;}
           IActorTransport Transport { get; }
           ICommandExecutor Executor { get; }
    }

    public static class DomainBuilderExtensions
    {
        public static HandlerRegistrator<INodeContext,TMessage, THandler> RegisterNodeHandler<TMessage, THandler>(this IDomainBuilder builder, Func<INodeContext, THandler> producer) where THandler : IHandler<TMessage>
                                                                                                                                                                               where TMessage : class, IHaveProcessId, IHaveId
        {
            return new HandlerRegistrator<INodeContext,TMessage, THandler>(producer, builder);
        }
    }
    
    public class DefaultNodeContext : INodeContext
    {
        public ICommandExecutor Executor { get; set; }
        public ActorSystem System { get;  set;}
        public IActorTransport Transport { get;  set;}
        public IPublisher Publisher => Transport;
        public ILogger Log { get;  set;}
    }
    
    public abstract class GridDomainNode : IGridDomainNode
    {
        private ICommandExecutor _commandExecutor
        {
            get => _context.Executor;
            set => _context.Executor = value;
        }
        
        private IMessageWaiterFactory _waiterFactory;
        protected internal IActorCommandPipe Pipe;

        protected abstract ICommandExecutor CreateCommandExecutor();
        protected abstract IActorCommandPipe CreateCommandPipe();
        protected abstract IActorTransport CreateTransport();
        
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

        public async Task Start()
        {
            Log.Information("Starting GridDomain node {Id}", Id);
            
            var domainBuilder = CreateDomainBuilder();

            await ConfigureDomain(domainBuilder);
            
            _commandExecutor = CreateCommandExecutor();

            BuildContainer(domainBuilder);
            
            await StartMessageRouting();

            await CreateControllerActor();

            Log.Information("GridDomain node {Id} started at home {Home}", Id, System.Settings.Home);
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

        protected virtual DomainBuilder CreateDomainBuilder()
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