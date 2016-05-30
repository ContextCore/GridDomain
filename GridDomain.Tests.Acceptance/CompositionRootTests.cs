using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    [TestFixture]
    public class CompositionRootTests
    {
        [TestCase(TransportMode.Cluster)]
        [TestCase(TransportMode.Standalone)]
        public void All_base_registrations_can_be_resolved(TransportMode transportMode)
        {
            var container = new UnityContainer();
            CompositionRoot.Init(container, 
                                 ActorSystemFactory.CreateActorSystem(
                                      new AutoTestAkkaConfiguration()),
                                     new LocalDbConfiguration(),
                                    transportMode);


            foreach (var reg in container.Registrations)
            {
                container.Resolve(reg.RegisteredType, reg.Name);
                Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
            }
        }

        [TestCase(TransportMode.Cluster)]
        [TestCase(TransportMode.Standalone)]
        public void All_registrations_can_be_resolved(TransportMode transportMode)
        {
            LocalDbConfiguration localDbConfiguration;
            var container = InitCoreContainer(transportMode, out localDbConfiguration);


            GridDomain.Balance.Node.CompositionRoot.Init(container, localDbConfiguration);

            foreach (var reg in container.Registrations.Where(r => !r.RegisteredType.Name.Contains("Actor")))
            {
                container.Resolve(reg.RegisteredType, reg.Name);
                Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
            }
        }

        private IDictionary<TransportMode, Func<ActorSystem>> ActorSystemBuilders = new Dictionary<TransportMode, Func<ActorSystem>>
            {
            {TransportMode.Standalone, () => ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration())}

            } } 
        private static UnityContainer InitCoreContainer(TransportMode transportMode,
                                                        IDbConfiguration localDbConfiguration)
        {
            var container = new UnityContainer();
            localDbConfiguration = new LocalDbConfiguration();

            if(transportMode = TransportMode.Standalone)

                CompositionRoot.Init(container,
                                     ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration()),
                                     localDbConfiguration,
                                     transportMode);
            return container;
        }
    }
}
