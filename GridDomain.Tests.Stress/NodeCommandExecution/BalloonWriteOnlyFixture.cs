using GridDomain.Tests.Unit;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class BalloonWriteOnlyFixture : NodeTestFixture
    {
        public BalloonWriteOnlyFixture(ITestOutputHelper helper):base(helper,new BallonWriteOnlyDomain())
        {
            NodeConfig = new StressTestNodeConfiguration();
            NodeConfig.LogLevel = LogEventLevel.Warning;
        }
    }
}