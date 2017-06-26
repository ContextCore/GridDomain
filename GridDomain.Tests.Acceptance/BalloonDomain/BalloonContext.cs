using System.Data.Entity;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext(string connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}