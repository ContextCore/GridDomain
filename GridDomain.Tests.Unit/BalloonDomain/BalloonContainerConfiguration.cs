using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class BalloonContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<Balloon, BalloonCommandHandler>();
            container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            container.RegisterType<BalloonCreatedNotificator>();
            container.RegisterType<BalloonTitleChangedNotificator>();
            container.RegisterType<BalloonCreatedFaultyProjection>();
        }
    }
}