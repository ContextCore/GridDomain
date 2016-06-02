using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();

            CompositionRoot.Init(container,
                                 ActorSystemBuilders[mode](),
                                 conf,
                                 mode);

            return container;
        }
    }
}