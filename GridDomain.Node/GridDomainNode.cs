using System;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
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
        public readonly IUnityContainer Container;
        public readonly ActorSystem System;

        public Guid Id { get; } = Guid.NewGuid();

        
        public GridDomainNode(IUnityContainer unityContainer, 
                              ActorSystem actorSystem)
        {
            Container = unityContainer;
            System = actorSystem;
        }

        public void Start(IDbConfiguration databaseConfiguration)
        {
            BusinessBalanceContext.DefaultConnectionString = databaseConfiguration.ReadModelConnectionString;
            ConfigureLog(databaseConfiguration);
            
            CompositionRoot.Init(Container,
                                 System,
                                 databaseConfiguration);

            //не убирать - нужен для работы DI в Akka
            var propsResolver = new UnityDependencyResolver(Container, System);
            StartActorSystem(System);
        }

        private static void ConfigureLog(IDbConfiguration dbConf)
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
            _mainNodeActor.Tell(new GridDomainNodeMainActor.Start());
            //TODO: replace with message wait
            Thread.Sleep(TimeSpan.FromSeconds(1));
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