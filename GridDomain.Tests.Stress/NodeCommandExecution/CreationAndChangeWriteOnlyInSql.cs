using GridDomain.Node;
using GridDomain.Tests.Acceptance.Snapshots;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class CreationAndChangeWriteOnlyInSql : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public CreationAndChangeWriteOnlyInSql(ITestOutputHelper output) : base(output)
        {
            _testOutputHelper = output;
        }
        protected override INodeScenario Scenario { get; } = new Stress.BalloonsCreationAndChangeScenario(20, 20);
        internal override IGridDomainNode CreateNode()
        {
            return new BalloonWriteOnlyFixture(_testOutputHelper).UseSqlPersistence().CreateNode().Result;
        }
    }
}