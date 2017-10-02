using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Unit;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture(DbContextOptions<BalloonContext> dbContextOptions)
        {
            Add(new BalloonWithProjectionDomainConfiguration(dbContextOptions));
        }
    }
}