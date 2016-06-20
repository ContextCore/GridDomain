using GridDomain.Scheduling.Integration;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestLoggerFactory : LoggerFactory
    {
        public override ISoloLogger GetLogger()
        {
            return new TestLogger();
        }
    }
}