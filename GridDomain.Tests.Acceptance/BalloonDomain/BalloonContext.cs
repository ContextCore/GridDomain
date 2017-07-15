using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext() : base(new DbContextOptionsBuilder().UseSqlServer(new AutoTestLocalDbConfiguration().ReadModelConnectionString).
                                                                     Options) { }

        public BalloonContext(DbContextOptions connString) : base(connString) { }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}