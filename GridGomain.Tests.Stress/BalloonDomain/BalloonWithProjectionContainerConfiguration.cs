using System;
using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using Microsoft.Practices.Unity;

namespace GridGomain.Tests.Stress.BalloonDomain
{
    public class BalloonWithProjectionContainerConfiguration : IContainerConfiguration
    {
        private readonly string _balloonConnString;
        public BalloonWithProjectionContainerConfiguration(string balloonConnString)
        {
            _balloonConnString = balloonConnString;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<Balloon, BalloonCommandHandler>();
            container.RegisterInstance<Func<BalloonContext>>(() => new BalloonContext(_balloonConnString));
            container.RegisterType<BalloonCatalogProjection>();
        }
    }
}