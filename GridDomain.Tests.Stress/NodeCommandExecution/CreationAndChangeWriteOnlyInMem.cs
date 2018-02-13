using GridDomain.Node;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class CreationAndChangeWriteOnlyInMem : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public CreationAndChangeWriteOnlyInMem(ITestOutputHelper output) : base(output)
        {
            _testOutputHelper = output;
        }
        protected override INodeScenario Scenario { get; } = new Stress.BalloonsCreationAndChangeScenario(10, 10);
        internal override IGridDomainNode CreateNode()
        {
            return new BalloonWriteOnlyFixture(_testOutputHelper).CreateNode().Result;
        }
        
    }
}