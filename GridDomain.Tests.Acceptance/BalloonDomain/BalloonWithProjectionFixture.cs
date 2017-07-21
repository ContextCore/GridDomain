using GridDomain.Tests.Acceptance.Projection;
using GridDomain.Tests.Unit;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonWithProjectionFixture : NodeTestFixture
    {
        public BalloonWithProjectionFixture(string dbConnectionString = null) 
        {
            Add(new BalloonWithProjectionDomainConfiguration(dbConnectionString ?? ConnectionStrings.AutoTestDb));
            this.ClearSheduledJobs();
        }
    }
}