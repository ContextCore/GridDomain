using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests;
using Microsoft.Practices.Unity;

namespace BusinessNews.Test
{
    public class BusinessNewsContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();
            var configuration = new GridNodeContainerConfiguration(ActorSystemBuilders[mode](), 
                                                                   conf,
                                                                   mode,
                                                                   new InMemoryQuartzConfig());

            container.Register(configuration);

            Node.CompositionRoot.Init(container, conf);
            return container;
        }
    }
}