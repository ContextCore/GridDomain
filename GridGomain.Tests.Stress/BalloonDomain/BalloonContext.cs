using Microsoft.EntityFrameworkCore;

namespace GridGomain.Tests.Stress.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext(DbContextOptions connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}