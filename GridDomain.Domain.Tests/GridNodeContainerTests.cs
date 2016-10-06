using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();

            CompositionRoot.Init(container, ActorSystemBuilders[mode](), mode);

            return container;
        }
    }
}