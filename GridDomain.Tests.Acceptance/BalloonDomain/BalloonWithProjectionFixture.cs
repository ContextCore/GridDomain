using System;
using GridDomain.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Unit;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture(ITestOutputHelper output,
                                            DbContextOptions<BalloonContext> dbContextOptions,
                                            NodeConfiguration cfg = null,
                                            Func<NodeConfiguration, string> configBuilder = null) :
            base(output, cfg, new IDomainConfiguration[] {
            new BalloonWithProjectionDomainConfiguration(dbContextOptions)
        })

        {
        }
    }
}