using Autofac;
using GridDomain.Configuration;

using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Node.Transports;
using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        private readonly ILogger _logger;

        public GridNodeContainerTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        protected override IContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new ContainerBuilder();
            var actorSystem = ActorSystemBuilders[mode]();
            container.Register(new GridNodeContainerConfiguration(new LocalAkkaEventBusTransport(actorSystem), _logger));
            actorSystem.Terminate();
            return container.Build();
        }
    }
}