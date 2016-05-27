using System;
using System.Linq;
using System.Threading;
using GridDomain.Balance.Domain;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;
using Topshelf;

namespace GridDomain.Balance.Node
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to GridDomain");
            Console.WriteLine("Launching GridDomain node");

            var container = new UnityContainer();
            var akkaConfig = container.Resolve<AkkaConfiguration>();
            var conf = new LocalDbConfiguration();
            CompositionRoot.Init(container,conf);

            ConfigureLog(conf);

            HostFactory.Run(x =>
            {
                x.Service<GridDomainNode>(s =>
                {
                    s.ConstructUsing(settings =>
                    {
                        var actorSystem = ActorSystemFactory.CreateCluster(akkaConfig,3,3).Last();
                        return new GridDomainNode(container, new BalanceCommandsRouting(), actorSystem);
                    });
                    s.WhenStarted(node =>
                    {
                        node.Start(conf);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        ApplySeeds(node);
                    });
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("GridDomain node host");
                x.SetDisplayName("GridDomain node");
                x.SetServiceName("GridDomain_node");
            });
            Console.ReadLine();
        }

        private static void ApplySeeds(GridDomainNode node)
        {
            foreach (var cmd in new SubscriptionsFeed().InitialSubscriptions())
            {
                node.Execute(cmd);
            }
        }

        private static void ConfigureLog(IDbConfiguration dbConf)
        {
            var conf = new LogConfigurator(new LoggingConfiguration());
            conf.InitDbLogging(LogLevel.Trace, dbConf.LogsConnectionString);
            conf.InitExternalLoggin(LogLevel.Trace);
            conf.InitConsole(LogLevel.Warn);
            conf.Apply();
        }
    }
}