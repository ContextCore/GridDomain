using GridDomain.Configuration;
using GridDomain.Tests.Unit;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class BalloonWriteOnlyFixture : NodeTestFixture
    {
        public BalloonWriteOnlyFixture(ITestOutputHelper helper):base(helper,
                                                                      new StressTestNodeConfiguration {LogLevel = LogEventLevel.Warning},
                                                                      null,
                                                                      new  IDomainConfiguration[]{new BallonWriteOnlyDomain()})
        {
        }
    }
}