using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;

namespace GridDomain.Node
{
    public class GridDomainNode : IGridDomainNode
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private IActorRef _mainNodeActor;
        public IUnityContainer Container { get; }
        public readonly ActorSystem[] AllSystems;
        private readonly IMessageRouteMap _messageRouting;
        private readonly TransportMode _transportMode;
        private static readonly IDictionary<TransportMode, Type> RoutingActorType = new Dictionary
           <TransportMode, Type>
        {
            {TransportMode.Standalone, typeof(LocalSystemRoutingActor)},
            {TransportMode.Cluster,    typeof(ClusterSystemRouterActor)}
        };

        public readonly ActorSystem System;

        public Guid Id { get; } = Guid.NewGuid();

        public GridDomainNode(IUnityContainer container, 
            IMessageRouteMap messageRouting, TransportMode transportMode, params ActorSystem[] actorAllSystems)
        {
            _transportMode = transportMode;
            _messageRouting = messageRouting;
            AllSystems = actorAllSystems;
            System = AllSystems.Last();
            Container = container;
        }

        public void Start(IDbConfiguration databaseConfiguration)
        {
            BusinessBalanceContext.DefaultConnectionString = databaseConfiguration.ReadModelConnectionString;
            ConfigureLog(databaseConfiguration);

            CompositionRoot.Init(Container,
                                System,
                                databaseConfiguration,
                                _transportMode);

            Container.RegisterInstance(_messageRouting);
            //не убирать - нужен для работы DI в Akka
            foreach (var system in AllSystems)
            {
                var propsResolver = new UnityDependencyResolver(Container, system);
            }

            StartActorSystem(System);
        }

        public static void ConfigureLog(IDbConfiguration dbConf)
        {
            var conf = new LogConfigurator(new LoggingConfiguration());
            conf.InitDbLogging(LogLevel.Trace, dbConf.LogsConnectionString);
            conf.InitExternalLoggin(LogLevel.Trace);
            conf.InitConsole(LogLevel.Warn);
            conf.Apply();
        }


        private void StartActorSystem(ActorSystem actorSystem)
        {
            _log.Info($"Launching GridDomain node {Id}");
            
            var props = actorSystem.DI().Props<GridDomainNodeMainActor>();
            _mainNodeActor = actorSystem.ActorOf(props);
            _mainNodeActor.Ask(new GridDomainNodeMainActor.Start()
            {
                RoutingActorType = RoutingActorType[_transportMode]
            })
            .Wait(TimeSpan.FromSeconds(10));

            _log.Info($"GridDomain node {Id} started at home '{actorSystem.Settings.Home}'");
        }

        public void Stop()
        {
            System.Terminate();
            System.Dispose();
            _log.Info($"GridDomain node {Id} stopped");
        }

        public void Execute(ICommand cmd)
        {
            _mainNodeActor.Tell(new GridDomainNodeMainActor.ExecuteCommand(cmd));
        }
    }
}