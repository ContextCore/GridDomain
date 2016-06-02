

using System;
using System.Configuration;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Akka.DI.Core;
using Akka.DI.Unity;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Samples.Cluster.Simple.Samples.Cluster.Simple;

namespace Samples.Cluster.Simple
{
    class Program
    {
        public static void Main(string[] args)
        {
            StartUp();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        public static void StartUp()
        {
            var ports = new []{"2551", "2552", "0","0","0"};
            var section = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");
            var container = new UnityContainer();
            container.RegisterInstance("key1");
            foreach (var port in ports)
            {
                //Override the configuration of the port
                var config =
                    ConfigurationFactory.ParseString("akka.remote.helios.tcp.port=" + port)
                        .WithFallback(section.AkkaConfig);

                //create an Akka system
                var system = ActorSystem.Create("ClusterSystem", config);
                system.AddDependencyResolver(new UnityDependencyResolver(container,system));
                //create an actor that handles cluster domain events
                system.ActorOf(system.DI().Props<SimpleClusterListener>());
            }
        }
    }

}