using GridDomain.Node;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit
{
    public class GridNodeContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();

            var actorSystem = ActorSystemBuilders[mode]();
            //container.RegisterInstance<IMessageWaiterFactory>();
            CompositionRoot.Init(container, actorSystem, mode);

            actorSystem.Terminate();
            return container;
        }
    }
}