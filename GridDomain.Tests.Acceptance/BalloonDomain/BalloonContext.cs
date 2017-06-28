using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext(DbContextOptions connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}