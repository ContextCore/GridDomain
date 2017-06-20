using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.BalloonDomain;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    public class BalloonWithProjectionContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<Balloon, BalloonCommandHandler>();
            container.RegisterType<BalloonCatalogProjection>();
        }
    }
}