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
        internal DbContextOptions<BalloonContext> DbContextOptions;

        public CreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            _testOutputHelper = output;
        }
        internal override void OnSetup()
        {
            DbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(CreationAndChangeWithProjectionInMem)).Options;
            using(var ctx = new BalloonContext(DbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 20);
        internal override IGridDomainNode CreateNode()
        {
            return new BalloonWithProjectionFixture(DbContextOptions)
                   {
                       Output = _testOutputHelper,
                       AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                       LogLevel = LogEventLevel.Error
                   }.CreateNode().Result;
        }

        protected override void OnCleanup()
        {
            using(var ctx = new BalloonContext(DbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
        }
    }
}