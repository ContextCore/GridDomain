using System;
using System.Threading;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;
using Topshelf;

namespace BusinessNews.Node
{
    /// <summary>
    ///     Business news is a news company who provide fresh news and articles for businessmen
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to GridDomain");
            Console.WriteLine("Launching GridDomain node");

            var container = new UnityContainer();
            var akkaConfig = container.Resolve<AkkaConfiguration>();
            var conf = new LocalDbConfiguration();
            CompositionRoot.Init(container, conf);

            HostFactory.Run(x =>
            {
                x.Service<GridDomainNode>(s =>
                {
                    s.ConstructUsing(settings =>
                    {
                        Func<ActorSystem[]> actorSystem = () => new [] { ActorSystemFactory.CreateCluster(akkaConfig).RandomNode()};
                        return new GridDomainNode(container, new BusinessNewsRouting(), actorSystem);
                    });
                    s.WhenStarted(node =>
                    {
                        node.Start(conf);
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
        }

        
    }
}