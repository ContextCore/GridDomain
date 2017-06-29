using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionDomainConfiguration : IDomainConfiguration
    {
        private readonly string _balloonConnString;

        public BalloonWithProjectionDomainConfiguration(string balloonConnString)
        {
            _balloonConnString = balloonConnString;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencyFactory());

            var options = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(_balloonConnString).Options;
            BalloonContext BalloonContextProducer () => new BalloonContext(options);

            var factoryA = new DefaultMessageHandlerFactory<BalloonTitleChanged, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log));
            var factoryB = new DefaultMessageHandlerFactory<BalloonCreated, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log));

            builder.RegisterHandler(factoryA);
            builder.RegisterHandler(factoryB);
        }
    }
}