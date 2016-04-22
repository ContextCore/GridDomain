using System;
using System.Linq;
using GridDomain.Balance.Commands;
using GridDomain.Balance.Domain;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;
using NMoneys;
using Topshelf;

namespace GridDomain.Node
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to GridDomain");
            Console.WriteLine("Launching GridDomain node");

            var container = new UnityContainer();
            // container.LoadConfiguration();
            var akkaConfig = container.Resolve<AkkaConfiguration>();
            var conf = new LocalDbConfiguration();
            ConfigureLog(conf);

            HostFactory.Run(x =>
            {
                x.Service<GridDomainNode>(s =>
                {
                    s.ConstructUsing(settings =>
                    {
                        var actorSystem = ActorSystemFactory.CreateCluster(akkaConfig).Last();
                        return new GridDomainNode(container, actorSystem);
                    });
                    s.WhenStarted(node =>
                    {
                        node.Start(conf);
                        OnStart(node);
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

        private static void ConfigureLog(IDbConfiguration dbConf)
        {
            var conf = new LogConfigurator(new LoggingConfiguration());
            conf.InitDbLogging(LogLevel.Trace, dbConf.LogsConnectionString);
            conf.InitExternalLoggin(LogLevel.Trace);
            conf.InitConsole(LogLevel.Warn);
            conf.Apply();
        }

        private static void OnStart(GridDomainNode node)
        {
            var balanceId = Guid.Parse("65D7AB60-D56D-4BD0-B9F8-031EB054CE38");
            var money = new Money(10, CurrencyIsoCode.RUB);
            var source = new BalanceChangeSource
            {
                Description = "test replenish",
                Id = Guid.NewGuid(),
                Name = "testSource"
            };
            node.Execute(new ReplenishBalanceCommand(balanceId, money, source));
        }
    }
}