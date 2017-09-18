using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tools.Repositories.RawDataRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress
{
    public class CommandExecutionWithProjectionPerfTestSql
    {
        private Counter _counter;
        private readonly ITestOutputHelper _testOutputHelper;
        private NodeTestFixture _fixture;
        private DbContextOptions<BalloonContext> _dbContextOptions;

        public CommandExecutionWithProjectionPerfTestSql(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _fixture = new BalloonWithProjectionFixture()
                      {
                          Output = _testOutputHelper,
                          AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                          LogLevel = LogEventLevel.Error
            }.UseSqlPersistence();

            var readDb = new AutoTestLocalDbConfiguration();
            _dbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(readDb.ReadModelConnectionString).Options;
            //warm up EF 
            using(var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = Guid.NewGuid(), LastChanged = DateTime.UtcNow, Title = "WarmUp" });
                ctx.SaveChanges();
            }

            TestDbTools.Truncate(_fixture.AkkaConfig.Persistence.JournalConnectionString,
                                 "BalloonCatalog")
                       .Wait();
           
            _fixture.CreateNode().Wait();

            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenarioProjection(2, 2);

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions with projections in sql",
                       NumberOfIterations = 3, 
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        //MAX: 400, need several launches to warm up sql server
        // 50 as test is run on 'slow' appveyor
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 50)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void MeasureCommandExecutionWithProjectionsInSql()
        {
            Scenario.Execute(_fixture.Node, p => _counter.Increment());
        }

        [PerfCleanup]
        public void Cleanup()
        {
            var totalCommandsToIssue = Scenario.CommandPlans.Count();
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(_fixture.AkkaConfig.Persistence.SnapshotConnectionString).Options;

            var rawJournalRepository = new RawJournalRepository(dbContextOptions);
            var count = rawJournalRepository.TotalCount();
            if (count != totalCommandsToIssue)
            {
                _fixture.Output.WriteLine($"!!! Journal contains only {count} of {totalCommandsToIssue} !!!");
                Task.Delay(2000).Wait();
                count = rawJournalRepository.TotalCount();
                _fixture.Output.WriteLine($"After 2 sec Journal contains {count} of {totalCommandsToIssue}");
            }

            using (var context = new BalloonContext(_dbContextOptions))
            {
                var projectedCount = context.BalloonCatalog.Select(x => x).Count();
                _fixture.Output.WriteLine($"Found {projectedCount} projected rows");
            }

            _fixture.Dispose();
        }
    }
}