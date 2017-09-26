using GridDomain.Tests.Unit;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class BalloonWriteOnlyFixture : NodeTestFixture
    {
        public BalloonWriteOnlyFixture(ITestOutputHelper helper):base(new BallonWriteOnlyDomain())
        {
            Output = helper;
            NodeConfig = new StressTestNodeConfiguration(Akka.Event.LogLevel.WarningLevel);
            LogLevel = LogEventLevel.Warning;
        }
    }
}