using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture(string dbConnectionString = null) : base()
        {
            Add(new BalloonWithProjectionDomainConfiguration(dbConnectionString ?? ConnectionStrings.AutoTestDb));
            this.ClearSheduledJobs();
        }
    }
}