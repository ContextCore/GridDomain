using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;
using IUnityContainer = Microsoft.Practices.Unity.IUnityContainer;

namespace GridDomain.Node
{ 

    public class GridDomainNode : IGridDomainNode
    {
        private static readonly IDictionary<TransportMode, Type> RoutingActorType = new Dictionary
            <TransportMode, Type>
        {
            {TransportMode.Standalone, typeof (LocalSystemRoutingActor)},
            {TransportMode.Cluster, typeof (ClusterSystemRouterActor)}
        };

        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IMessageRouteMap _messageRouting;
        private readonly TransportMode _transportMode;
        public readonly ActorSystem[] AllSystems;
        private Quartz.IScheduler _quartzScheduler;
        public readonly ActorSystem System;
        private IActorRef _mainNodeActor;
        private readonly IContainerConfiguration _configuration;
        private readonly IQuartzConfig _quartzConfig;
        public IPublisher Transport { get; private set; }

        public GridDomainNode(IUnityContainer container,
                              IMessageRouteMap messageRouting,
                              TransportMode transportMode,
                              IQuartzConfig quartzConfig = null,
                              params ActorSystem[] actorAllSystems)
            : this(new EmptyContainerConfig(),messageRouting,transportMode, quartzConfig,actorAllSystems)
        {
            Container = container;
        }
   
        //for backward compatibility
        public GridDomainNode(IUnityContainer container,
                             IMessageRouteMap messageRouting,
                             TransportMode transportMode,
                             params ActorSystem[] actorAllSystems)
           : this(container, messageRouting, transportMode, null, actorAllSystems)
        {
        }

        //for backward compatibility
        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              TransportMode transportMode,
                              params ActorSystem[] actorAllSystems):this(configuration, messageRouting, transportMode, null, actorAllSystems)
        {
        }

        public GridDomainNode(IContainerConfiguration configuration,
                              IMessageRouteMap messageRouting,
                              TransportMode transportMode,
                              IQuartzConfig quartzConfig = null,
                              params ActorSystem[] actorAllSystems)
        {
            _quartzConfig = quartzConfig ?? new InMemoryQuartzConfig();
            _configuration = configuration;
            _transportMode = transportMode;
            _messageRouting = new CompositeRouteMap(messageRouting, new SchedulingRouteMap());
            AllSystems = actorAllSystems;
            Container = new UnityContainer();
            System = AllSystems.First();
            System.WhenTerminated.ContinueWith(OnSystemTermination);
            System.RegisterOnTermination(OnSystemTermination);
        }
        private void OnSystemTermination()
        {
            _log.Debug("grid node Actor system terminated");
        }
        private void OnSystemTermination(Task obj)
        {
            _log.Debug("grid node Actor system terminated");
        }

        public IUnityContainer Container { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public void Start(IDbConfiguration databaseConfiguration)
        {
            System.AddDependencyResolver(new UnityDependencyResolver(Container, System));
            ConfigureContainer(Container,databaseConfiguration, _quartzConfig, System);

            StartMainNodeActor(System);

            Transport = Container.Resolve<IPublisher>();
            _quartzScheduler = Container.Resolve<Quartz.IScheduler>();
        }

        private void ConfigureContainer(IUnityContainer unityContainer,
                                        IDbConfiguration databaseConfiguration, 
                                        IQuartzConfig quartzConfig, 
                                        ActorSystem actorSystem)
        {
            unityContainer.Register(new GridNodeContainerConfiguration(actorSystem,
                                                                       databaseConfiguration,
                                                                       _transportMode,
                                                                       quartzConfig));
            
            var persistentScheduler = actorSystem.ActorOf(System.DI().Props<SchedulingActor>());
            unityContainer.RegisterInstance(new TypedMessageActor<ScheduleMessage>(persistentScheduler));
            unityContainer.RegisterInstance(new TypedMessageActor<ScheduleCommand>(persistentScheduler));
            unityContainer.RegisterInstance(new TypedMessageActor<Unschedule>(persistentScheduler));
            unityContainer.RegisterInstance(_messageRouting);
            _configuration.Register(unityContainer);
        }

        public void Stop()
        {
            _quartzScheduler.Shutdown(false);
            System.Terminate();
            System.Dispose();

            _log.Info($"GridDomain node {Id} stopped");
        }

        private void StartMainNodeActor(ActorSystem actorSystem)
        {
            _log.Info($"Launching GridDomain node {Id}");

            var props = actorSystem.DI().Props<GridDomainNodeMainActor>();
            _mainNodeActor = actorSystem.ActorOf(props,nameof(GridDomainNodeMainActor));
            _mainNodeActor.Ask(new GridDomainNodeMainActor.Start
            {
                RoutingActorType = RoutingActorType[_transportMode]
            })
            .Wait(TimeSpan.FromSeconds(2));

            _log.Info($"GridDomain node {Id} started at home '{actorSystem.Settings.Home}'");
        }

        public void Execute(params ICommand[] commands)
        {
            foreach(var cmd in commands)
                _mainNodeActor.Tell(cmd);
        }

        public Task<object> Execute(ICommand command, params ExpectedMessage[] expect)
        {
            return _mainNodeActor.Ask<object>(new CommandAndConfirmation(command,expect))
                                 .ContinueWith(t =>
                                 {
                                     object result=null;
                                     t.Result.Match()
                                         .With<ICommandFault>(fault =>
                                         {
                                             var domainExcpetion = fault.Exception.UnwrapSingle();
                                             ExceptionDispatchInfo.Capture(domainExcpetion).Throw();
                                         })
                                         .With<CommandExecutionFinished>(finish => result = finish.ResultMessage)
                                         .Default(m => { throw new InvalidMessageException(m.ToPropsString()); }); 

                                     return result;
                                 });
        }
    }
}