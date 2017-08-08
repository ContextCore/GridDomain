using GridDomain.Configuration;
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

            builder.RegisterHandler<BalloonTitleChanged, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log))
                   .AsSync();

            builder.RegisterHandler<BalloonCreated, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log))
                   .AsSync();
        }
    }
}