using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    [TestFixture]
    public abstract class CompositionRootTests
    {
        [TestCase(TransportMode.Cluster)]
        [TestCase(TransportMode.Standalone)]
        public void All_base_registrations_can_be_resolved(TransportMode transportMode)
        {
            var container = CreateContainer(transportMode, new LocalDbConfiguration());
            ResolveAll(container);
        }

        protected abstract IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf);

        private void ResolveAll(IUnityContainer container)
        {
            var errors = new Dictionary<ContainerRegistration, Exception>();
            foreach (var reg in container.Registrations.Where(r => !r.RegisteredType.Name.Contains("Actor")))
            {
                try
                {
                    container.Resolve(reg.RegisteredType, reg.Name);
                    Console.WriteLine($"resolved {reg.RegisteredType} {reg.Name}");
                }
                catch (Exception ex)
                {
                    errors[reg] = ex;
                }
            }

            if (!errors.Any()) return;

            foreach (var error in errors.Take(5))
            {
                Console.WriteLine();
                Console.WriteLine($"Exception while resolving {error.Key.RegisteredType} {error.Key.Name}");
                Console.WriteLine();
            }
            Assert.Fail("Found unresolvable registrations");
        }

        protected  readonly IDictionary<TransportMode, Func<ActorSystem>> ActorSystemBuilders = new Dictionary
            <TransportMode, Func<ActorSystem>>
        {
            {TransportMode.Standalone, () => ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration())},
            {
                TransportMode.Cluster, () => ActorSystemFactory.CreateCluster(new AutoTestAkkaConfiguration()).RandomNode()
            }
        };

  
    }
}