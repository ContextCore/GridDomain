using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tools.Repositories.RawDataRepositories;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class CreationAndChangeWithProjectionInSql : ScenarionPerfTest
    {
        private readonly ITestOutputHelper _output;
        internal DbContextOptions<BalloonContext> DbContextOptions;

        public CreationAndChangeWithProjectionInSql(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }

        internal override void OnSetup()
        {
            var readDb = new AutoTestLocalDbConfiguration();
            DbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(readDb.ReadModelConnectionString).Options;
            using(var ctx = new BalloonContext(DbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = Guid.NewGuid(), LastChanged = DateTime.UtcNow, Title = "WarmUp" });
                ctx.SaveChanges();
            }
            base.OnSetup();
        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 50);
        internal override IGridDomainNode CreateNode()
        {
            var fixture = new BalloonWithProjectionFixture(DbContextOptions)
                                               {
                                                   Output = _output,
                                                   NodeConfig = new StressTestNodeConfiguration(),
                                                   LogLevel = LogEventLevel.Error
                                               }.UseSqlPersistence();

            fixture.SystemConfigFactory = () => fixture.NodeConfig.ToStandAloneSystemConfig(AutoTestNodeDbConfiguration.Default);
            return fixture.CreateNode().Result;
        }

        public override void Cleanup()
        {
            var totalCommandsToIssue = Scenario.CommandPlans.Count();
            var journalConnectionString = new AutoTestNodeDbConfiguration().JournalConnectionString;
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(journalConnectionString).Options;

            var rawJournalRepository = new RawJournalRepository(dbContextOptions);
            var count = rawJournalRepository.TotalCount();
            if(count != totalCommandsToIssue)
            {
                _output.WriteLine($"!!! Journal contains only {count} of {totalCommandsToIssue} !!!");
                Task.Delay(2000).Wait();
                count = rawJournalRepository.TotalCount();
                _output.WriteLine($"After 2 sec Journal contains {count} of {totalCommandsToIssue}");
            }

            using(var context = new BalloonContext(DbContextOptions))
            {
                var projectedCount = context.BalloonCatalog.Select(x => x).Count();
                _output.WriteLine($"Found {projectedCount} projected rows");
            }
        }
    }
}