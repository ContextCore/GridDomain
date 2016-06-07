using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests;
using Microsoft.Practices.Unity;

namespace BusinessNews.Test
{
    public class BusinessNewsContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();
            CompositionRoot.Init(container,
                ActorSystemBuilders[mode](),
                conf,
                mode);
            Node.CompositionRoot.Init(container, conf);
            return container;
        }
    }
}