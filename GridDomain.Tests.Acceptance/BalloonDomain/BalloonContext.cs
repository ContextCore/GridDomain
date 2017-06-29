using GridDomain.Tests.Common.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext() : base(new DbContextOptionsBuilder().UseSqlServer(new AutoTestAkkaConfiguration().Persistence.JournalConnectionString).
                                                                     Options) { }

        public BalloonContext(DbContextOptions connString) : base(connString) { }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}