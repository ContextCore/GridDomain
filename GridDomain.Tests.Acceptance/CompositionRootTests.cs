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

            foreach (var reg in container.Registrations.Where(r => !r.RegisteredType.Name.Contains("Actor")))
            {
                container.Resolve(reg.RegisteredType, reg.Name);
                Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
            }
        }
    }
}
