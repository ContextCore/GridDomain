using GridDomain.Node;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class CreationAndChangeWriteOnly_CommandAggregate_InMem : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public CreationAndChangeWriteOnly_CommandAggregate_InMem(ITestOutputHelper output) : base(output)
        {
            _testOutputHelper = output;
        }
        protected override INodeScenario Scenario { get; } = new HomeCreationAndChangeScenario(10, 10);
        internal override IGridDomainNode CreateNode()
        {
            var nodeTestFixture = new NodeTestFixture(_testOutputHelper, new SoftwareDomainConfiguration()) {NodeConfig = {LogLevel = LogEventLevel.Warning}};
            return nodeTestFixture.CreateNode().Result;
        }

    }
}