using System.Data.Entity;

namespace GridGomain.Tests.Stress.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext(string connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}