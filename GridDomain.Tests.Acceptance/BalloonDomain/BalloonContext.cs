using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonContext : DbContext
    {
       public BalloonContext() : base(
           new DbContextOptionsBuilder().UseSqlServer(new AutoTestLocalDbConfiguration().ReadModelConnectionString).
                                         Options)
       {
           Database.Migrate();
       }

        public BalloonContext(DbContextOptions connString) : base(connString)
        {
             Database.EnsureCreated();
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}