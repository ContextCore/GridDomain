using System.Data.Entity;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    public class BallonContext : DbContext
    {
        public BallonContext(string connString):base(connString)
        {
            
        }

        public DbSet<BalloonCatalogItem> BalloonCatalog { get; set; }
    }
}