using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Unit;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class NodeCommandExecutionInMem : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public NodeCommandExecutionInMem(ITestOutputHelper output) : base(output)
        {
            _testOutputHelper = output;
        }
        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 20);
        protected override IGridDomainNode CreateNode()
        {
            return new BalloonFixture
                   {
                       Output = _testOutputHelper,
                       AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                       LogLevel = LogEventLevel.Error
                   }
                .CreateNode().Result;
        }
    }
}