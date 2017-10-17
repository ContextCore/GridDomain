using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Unit;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture(ITestOutputHelper output, DbContextOptions<BalloonContext> dbContextOptions):
            base(output,new BalloonWithProjectionDomainConfiguration(dbContextOptions))
        {
        }
    }
}