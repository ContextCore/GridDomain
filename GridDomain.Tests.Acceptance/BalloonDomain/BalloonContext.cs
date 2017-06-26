using System.Data.Entity;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    public class BalloonContext : DbContext
    {
        public BalloonContext(string connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}