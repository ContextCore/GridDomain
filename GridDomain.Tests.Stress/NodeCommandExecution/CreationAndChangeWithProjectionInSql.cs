using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.Node;
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
        private DbContextOptions<BalloonContext> _dbContextOptions;

        public CreationAndChangeWithProjectionInSql(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }
        protected override void OnSetup()
        {
            var readDb = new AutoTestLocalDbConfiguration();
            _dbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(readDb.ReadModelConnectionString).Options;
            using(var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }
            //warm up EF 
            using(var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = Guid.NewGuid(), LastChanged = DateTime.UtcNow, Title = "WarmUp" });
                ctx.SaveChanges();
            }

        }

        protected override INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(20, 20);
        protected override IGridDomainNode CreateNode()
        {
            return new BalloonWithProjectionFixture(_dbContextOptions)
                   {
                       Output = _output,
                       AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                       LogLevel = LogEventLevel.Error
                   }.UseSqlPersistence().CreateNode().Result;
        }

        public override void Cleanup()
        {
            var totalCommandsToIssue = Scenario.CommandPlans.Count();
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(new AutoTestAkkaDbConfiguration().JournalConnectionString).Options;

            var rawJournalRepository = new RawJournalRepository(dbContextOptions);
            var count = rawJournalRepository.TotalCount();
            if(count != totalCommandsToIssue)
            {
                _output.WriteLine($"!!! Journal contains only {count} of {totalCommandsToIssue} !!!");
                Task.Delay(2000).Wait();
                count = rawJournalRepository.TotalCount();
                _output.WriteLine($"After 2 sec Journal contains {count} of {totalCommandsToIssue}");
            }

            using(var context = new BalloonContext(_dbContextOptions))
            {
                var projectedCount = context.BalloonCatalog.Select(x => x).Count();
                _output.WriteLine($"Found {projectedCount} projected rows");
            }

            base.Cleanup();
        }
    }
}