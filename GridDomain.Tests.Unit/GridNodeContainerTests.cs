using System;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();

            var actorSystem = ActorSystemBuilders[mode]();
            container.Register(new GridNodeContainerConfiguration(actorSystem, mode, new InMemoryQuartzConfig(), TimeSpan.FromSeconds(10)));
            actorSystem.Terminate();
            return container;
        }
    }
}