using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Projection
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