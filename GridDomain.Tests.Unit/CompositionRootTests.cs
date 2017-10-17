using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Autofac.Core;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using Xunit;

namespace GridDomain.Tests.Unit
{
    public abstract class CompositionRootTests
    {
        protected readonly IDictionary<TransportMode, Func<ActorSystem>> ActorSystemBuilders =
            new Dictionary<TransportMode, Func<ActorSystem>>
            {
                {
                    TransportMode.Standalone,
                    () => new AutoTestNodeConfiguration().CreateInMemorySystem()
                }
            };

        [Theory]
        [InlineData(TransportMode.Standalone)]
        public void All_base_registrations_can_be_resolved(TransportMode transportMode)
        {
            var container = CreateContainer(transportMode, new AutoTestLocalDbConfiguration());
            ResolveAll(container);
        }

        [Theory]
        [InlineData(TransportMode.Standalone)]
        public void Container_can_be_disposed(TransportMode transportMode)
        {
            var createContainer = Task.Run(() => CreateContainer(transportMode, new AutoTestLocalDbConfiguration()));
            if (!createContainer.Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Container creation took to much time");

            var container = createContainer.Result;

            if (!Task.Run(() => container.Dispose())
                     .Wait(TimeSpan.FromSeconds(5)))
                throw new TimeoutException("Container dispose took too much time");

            Console.WriteLine("Container disposed");
        }

        protected abstract IContainer CreateContainer(TransportMode mode, IDbConfiguration conf);

        //TODO: add named services resolution
        private void ResolveAll(IContainer container)
        {
            Console.WriteLine();
            var errors = new Dictionary<IServiceWithType, Exception>();
            foreach (var reg in container.ComponentRegistry.Registrations.SelectMany(x => x.Services)
                                                                         .OfType<IServiceWithType>()
                                                                         .Where(r => !r.ServiceType.Name.Contains("Actor")))
                try
                {
                    container.Resolve(reg.ServiceType);
                    Console.WriteLine($"resolved {reg.ServiceType}");
                }
                catch (Exception ex)
                {
                    errors[reg] = ex;
                }

            Console.WriteLine();
            if (!errors.Any())
                return;

            var builder = new StringBuilder();
            foreach (var error in errors.Take(5))
                builder.AppendLine($"Exception while resolving {error.Key.ServiceType} : {error.Value}");

            Assert.True(false, "Can not resolve registrations: \r\n " + builder);
        }
    }
}