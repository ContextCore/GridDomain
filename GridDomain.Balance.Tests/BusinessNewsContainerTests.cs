using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests;
using Microsoft.Practices.Unity;
using CompositionRoot = BusinessNews.Node.CompositionRoot;

namespace BusinessNews.Test
{
    public class BusinessNewsContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();
            GridDomain.Node.CompositionRoot.Init(container,
                                            ActorSystemBuilders[mode](),
                                            conf,
                                            mode);
            CompositionRoot.Init(container, conf);
            return container;
        }
    }
}