using System.Diagnostics;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.BalloonDomain;
using Microsoft.EntityFrameworkCore;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class NodeCommandExecutionWithProjectionInMem : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private DbContextOptions<BalloonContext> _dbContextOptions;

        public NodeCommandExecutionWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            _testOutputHelper = output;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }
        protected override void OnSetup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(NodeCommandExecutionWithProjectionInMem)).Options;
            using(var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenarioProjection(10, 10);
        protected override IGridDomainNode CreateNode()
        {
            return new BalloonWithProjectionFixture(_dbContextOptions)
                   {
                       Output = _testOutputHelper,
                       AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                       LogLevel = LogEventLevel.Error
                   }.CreateNode().Result;
        }
    }
}