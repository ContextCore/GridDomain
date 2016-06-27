using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;

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

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IMessageRouteMap _messageRouting;
        private readonly TransportMode _transportMode;
        public readonly ActorSystem[] AllSystems;

        public readonly ActorSystem System;
        private IActorRef _mainNodeActor;

        public GridDomainNode(IUnityContainer container,
            IMessageRouteMap messageRouting,
            TransportMode transportMode,
            params ActorSystem[] actorAllSystems)
        {
            _transportMode = transportMode;
            _messageRouting = messageRouting;
            AllSystems = actorAllSystems;
            System = AllSystems.Last();
            Container = container;
        }

        public IUnityContainer Container { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public void Start(IDbConfiguration databaseConfiguration)
        {
            ConfigureLog(databaseConfiguration);
            Container.RegisterInstance(_messageRouting);

            foreach (var system in AllSystems)
            {
                var r = new UnityDependencyResolver(Container, system);
               // system.AddDependencyResolver(new UnityDependencyResolver(Container, system));
                CompositionRoot.Init(Container.CreateChildContainer(),
                    system,
                    databaseConfiguration,
                    _transportMode);
            }
            CompositionRoot.Init(Container,
                System,
                databaseConfiguration,
                _transportMode);

            StartMainNodeActor(System);
        }

        public void Stop()
        {
            System.Terminate();
            System.Dispose();
            _log.Info($"GridDomain node {Id} stopped");
        }

        public static void ConfigureLog(IDbConfiguration dbConf)
        {
            var conf = new LogConfigurator(new LoggingConfiguration());
            conf.InitDbLogging(LogLevel.Trace, dbConf.LogsConnectionString);
            conf.InitExternalLoggin(LogLevel.Trace);
            conf.InitConsole(LogLevel.Warn);
            conf.Apply();
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
                 _mainNodeActor.Tell(new GridDomainNodeMainActor.ExecuteCommand(cmd));
        }

        //public ICommandStatus ExecuteTracking(ICommand command)
        //{
        //    throw new NotImplementedException();
        //}
    }
}