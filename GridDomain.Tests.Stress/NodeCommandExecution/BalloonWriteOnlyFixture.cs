using GridDomain.Configuration;
using GridDomain.Tests.Unit;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class BalloonWriteOnlyFixture : NodeTestFixture
    {
        public BalloonWriteOnlyFixture(ITestOutputHelper output):base(output,
                                                                      new StressTestNodeConfiguration {LogLevel = LogEventLevel.Warning},
                                                                      new  IDomainConfiguration[]{new BallonWriteOnlyDomain()})
        {
        }
    }
}