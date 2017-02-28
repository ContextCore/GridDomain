using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;
using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        private readonly ILogger _logger;

        public GridNodeContainerTests(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();

            var actorSystem = ActorSystemBuilders[mode]();
            container.Register(new GridNodeContainerConfiguration(actorSystem, mode, new NodeSettings()));
            actorSystem.Terminate();
            return container;
        }
    }
}