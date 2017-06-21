using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tools.Repositories.RawDataRepositories;
using GridGomain.Tests.Stress.BalloonDomain;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridGomain.Tests.Stress
{
    public class CommandExecutionWithProjectionPerfTest
    {
        private Counter _counter;
        private NodeTestFixture Fixture { get; }

        public CommandExecutionWithProjectionPerfTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));

            Fixture = new BalloonWithProjectionFixture()
                      {
                          Output = output,
                          InMemory = false,
                          AkkaConfig = new StressTestAkkaConfiguration(LogLevel.WarningLevel),
                          LogLevel = LogEventLevel.Warning
                      };
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            //warm up EF 
            using (var ctx = new BalloonContext(Fixture.AkkaConfig.Persistence.JournalConnectionString))
            {
                ctx.BalloonCatalog.Add(new BalloonCatalogItem() { BalloonId = Guid.NewGuid(), LastChanged = DateTime.UtcNow, Title = "WarmUp" });
                ctx.SaveChanges();
            }

            //Database.SetInitializer(new CreateDatabaseIfNotExists<BalloonContext>());
            TestDbTools.Truncate(Fixture.AkkaConfig.Persistence.JournalConnectionString,
                                 "BalloonCatalogitems")
                       .Wait();

            Fixture.CreateNode().Wait();

            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenarioProjection(50, 5);

        [NBenchFact]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 10, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 10000, TestMode = TestMode.Test)]
        //MAX: 400, need several launches to warm up sql server
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 300)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void Benchmark()
        {
            Scenario.Execute(Fixture.Node, p => _counter.Increment());
        }

        [PerfCleanup]
        public void Cleanup()
        {
            var totalCommandsToIssue = Scenario.CommandPlans.Count();

            var rawJournalRepository = new RawJournalRepository(Fixture.AkkaConfig.Persistence.JournalConnectionString);
            var count = rawJournalRepository.TotalCount();
            if (count != totalCommandsToIssue)
            {
                Fixture.Output.WriteLine($"!!! Journal contains only {count} of {totalCommandsToIssue} !!!");
                Task.Delay(2000).Wait();
                count = rawJournalRepository.TotalCount();
                Fixture.Output.WriteLine($"After 2 sec Journal contains {count} of {totalCommandsToIssue}");
            }

            using (var context = new BalloonContext(Fixture.AkkaConfig.Persistence.JournalConnectionString))
            {
                var projectedCount = context.BalloonCatalog.Select(x => x).Count();
                Fixture.Output.WriteLine($"Found {projectedCount} projected rows");
            }

            Fixture.Dispose();
        }
    }
}