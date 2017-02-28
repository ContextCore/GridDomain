using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using Xunit;

namespace GridDomain.Tests.XUnit
{
    public abstract class CompositionRootTests
    {
        protected readonly IDictionary<TransportMode, Func<ActorSystem>> ActorSystemBuilders =
            new Dictionary<TransportMode, Func<ActorSystem>>
            {
                {
                    TransportMode.Standalone,
                    () => new AutoTestAkkaConfiguration().CreateSystem()
                },
                {
                    TransportMode.Cluster,
                    () =>
                    ActorSystemFactory.CreateCluster(
                        new AutoTestAkkaConfiguration()).RandomNode()
                }
            };

        [Theory]
        [InlineData(TransportMode.Standalone)]
        public void All_base_registrations_can_be_resolved(TransportMode transportMode)
        {
            var container = CreateContainer(transportMode, new LocalDbConfiguration());
            ResolveAll(container);
        }

        [Theory]
        [InlineData(TransportMode.Standalone)]
        public void Container_can_be_disposed(TransportMode transportMode)
        {
            var createContainer = Task.Run(() => CreateContainer(transportMode, new LocalDbConfiguration()));
            if (!createContainer.Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Container creation took to much time");

            var container = createContainer.Result;

            var registrations = container.Registrations.ToArray();

            foreach (var reg in registrations)
            {
                Console.WriteLine("Registration");
                Console.WriteLine(reg.Name);
                Console.WriteLine(reg.MappedToType);
                Console.WriteLine(reg.RegisteredType);
                Console.WriteLine("end of registration");
                Console.WriteLine();
            }


            if (!Task.Run(() => container.Dispose()).Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Container dispose took too much time");

            Console.WriteLine("Container disposed");
        }

        protected abstract IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf);

        private void ResolveAll(IUnityContainer container)
        {
            Console.WriteLine();
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

            Console.WriteLine();
            if (!errors.Any()) return;

            var builder = new StringBuilder();
            foreach (var error in errors.Take(5))
            {
                builder.AppendLine($"Exception while resolving {error.Key.RegisteredType} {error.Key.Name} : {error.Value}");
            }

            Assert.True(false, "Can not resolve registrations: \r\n " + builder);
        }
    }
}