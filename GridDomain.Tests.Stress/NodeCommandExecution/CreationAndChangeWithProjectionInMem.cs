using System.Diagnostics;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
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
            DbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(CreationAndChangeWithProjectionInMem)).Options;
        }
        internal override void OnSetup()
        {
            using(var ctx = new BalloonContext(DbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
            base.OnSetup();
        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 20);
        internal override IGridDomainNode CreateNode()
        {
            var akkaConfig = new StressTestNodeConfiguration();
            return new BalloonWithProjectionFixture(_testOutputHelper,DbContextOptions)
                   {
                       NodeConfig = akkaConfig,
                       LogLevel = LogEventLevel.Error,
                       SystemConfigFactory = () => akkaConfig.ToStandAloneInMemorySystemConfig()
            }.CreateNode().Result;
        }
    }
}