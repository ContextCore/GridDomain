using System;
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
        private readonly IUnityContainer _container;
        private readonly AkkaConfiguration _akkaConf;
        private readonly IDbConfiguration _databaseConfiguration;
        public ActorSystem System;

        public Guid Id { get; } = Guid.NewGuid();

        public GridDomainNode(AkkaConfiguration akkaConf,
                            IDbConfiguration databaseConfiguration,
                            IUnityContainer unityContainer)
        {
            _akkaConf = akkaConf;
            _databaseConfiguration = databaseConfiguration;
            _container = unityContainer;

        }

        public void Start()
        {
            ConfigureNode(_databaseConfiguration, _akkaConf);
        }

        private static void ConfigureLog(IDbConfiguration dbConf)
        {
            var conf = new LogConfigurator(new LoggingConfiguration());
            conf.InitDbLogging(LogLevel.Trace, dbConf.LogsConnectionString);
            conf.InitExternalLoggin(LogLevel.Trace);
            conf.InitConsole(LogLevel.Warn);
            conf.Apply();
        }

        private void ConfigureNode(IDbConfiguration dbConf, 
                                   AkkaConfiguration akkaConf)
        {
           BusinessBalanceContext.DefaultConnectionString = dbConf.ReadModelConnectionString;
            ConfigureLog(dbConf);
            _log.Info($"Launching GridDomain node {Id}");

            //TODO: придумать как сделать конфиг через человеческий класс
            var actorSystem = ActorSystem.Create(akkaConf.Name,
                @"akka {  
                        actor {
                                 provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
                                 loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                                 debug {
                                          receive = on
                                          autoreceive = on
                                          lifecycle = on
                                          event-stream = on
                                          unhandled = on
                                       }
                        }
                        stdout-loglevel = ERROR
                        loglevel = ERROR
                        log-config-on-start = on


                        cluster {
                                seed-nodes = ""akka.tcp://" + akkaConf.Name + "@"+akkaConf.Host+":" + akkaConf.Port + @"""
                            }
                        remote {
                                    helios.tcp {
                                        transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                        transport-protocol = tcp
                                        port = " + akkaConf.Port+ @"}
                                        hostname = "+akkaConf.Host+ @"/
                                       
                                    }
                                }
                       ");

            //не убирать - нужен для работы DI в Akka
            System = actorSystem;
            var propsResolver = new UnityDependencyResolver(_container, System);
            CompositionRoot.Init(_container,
                     System,
                     _databaseConfiguration);

            var props = System.DI().Props<GridDomainNodeMainActor>();
            _mainNodeActor = System.ActorOf(props);
            _mainNodeActor.Ask(new GridDomainNodeMainActor.Start()).Wait();

            _log.Info($"GridDomain node {Id} started at home '{System.Settings.Home}'");
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

        public void ExecuteSync(ICommand cmd)
        {
            _mainNodeActor.Ask(new GridDomainNodeMainActor.ExecuteCommand(cmd)).Wait();
        }
    }
}