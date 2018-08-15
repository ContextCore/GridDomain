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
        private readonly DbContextOptions<BalloonContext> _dbContextOptions;

        public BalloonWithProjectionDomainConfiguration(DbContextOptions<BalloonContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new BalloonDependencies());

            BalloonContext BalloonContextProducer () => new BalloonContext(_dbContextOptions);

            builder.RegisterHandler<BalloonTitleChanged, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log))
                   .AsSync();

            builder.RegisterHandler<BalloonCreated, BalloonCatalogProjection>(c => new BalloonCatalogProjection(BalloonContextProducer, c.Publisher, c.Log))
                   .AsSync();
        }
    }
}