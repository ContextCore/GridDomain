using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit.Cluster;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Projection
{
    public class Cluster_BalloonEventsShouldBeProjected : BalloonEventsShouldBeProjected
    {
        public Cluster_BalloonEventsShouldBeProjected(ITestOutputHelper output) :
            base(new BalloonWithProjectionFixture(output,
                                                  new DbContextOptionsBuilder<BalloonContext>()
                                                      .UseSqlServer(ConnectionStrings.AutoTestDb)
                                                      .Options).UseSqlPersistence()
                                                               .Clustered()) { }
    }
}