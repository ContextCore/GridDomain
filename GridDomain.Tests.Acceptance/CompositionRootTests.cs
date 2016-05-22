using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{

    class AutoTestAkkaNetworkConfiguration: IAkkaNetworkConfiguration
    {
        public string Name => "LocalSystem";
        public string Host => "127.0.0.1";
        public int PortNumber => 8080;
    }

    class AutoTestAkkaConfiguration : AkkaConfiguration
    {
        public AutoTestAkkaConfiguration():base(new AutoTestAkkaNetworkConfiguration(), 
                                                new AutoTestAkkaDbConfiguration(),
                                                LogVerbosity.Warning)
        {
            
        }
    }

    [TestFixture]
    public class CompositionRootTests
    {
        [Test]
        public void All_base_registrations_can_be_resolved()
        {
            var container = new UnityContainer();
            CompositionRoot.Init(container, 
                                 ActorSystemFactory.CreateActorSystem(
                                      new AutoTestAkkaConfiguration()),
                                 new LocalDbConfiguration());


            foreach (var reg in container.Registrations)
            {
                container.Resolve(reg.RegisteredType, reg.Name);
                Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
            }
        }

        [Test]
        public void All_registrations_can_be_resolved()
        {
            var container = new UnityContainer();
            var localDbConfiguration = new LocalDbConfiguration();

            CompositionRoot.Init(container,
                                 ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration()),
                                 localDbConfiguration);


            GridDomain.Balance.Node.CompositionRoot.Init(container, localDbConfiguration);

            foreach (var reg in container.Registrations)
            {
                container.Resolve(reg.RegisteredType, reg.Name);
                Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
            }
        }
    }
}
