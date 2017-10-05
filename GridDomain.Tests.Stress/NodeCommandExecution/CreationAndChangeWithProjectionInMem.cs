using System.Diagnostics;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.BalloonDomain;
using Microsoft.EntityFrameworkCore;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class CreationAndChangeWithProjectionInMem : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private DbContextOptions<BalloonContext> _dbContextOptions;

        public CreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            _testOutputHelper = output;
        }
        protected override void OnSetup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(CreationAndChangeWithProjectionInMem)).Options;
            using(var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 20);
        internal override IGridDomainNode CreateNode()
        {
            return new BalloonWithProjectionFixture(_dbContextOptions)
                   {
                       Output = _testOutputHelper,
                       NodeConfig = new StressTestNodeConfiguration(LogLevel.ErrorLevel),
                       LogLevel = LogEventLevel.Error
                   }.CreateNode().Result;
        }
    }
}