using System.Diagnostics;
using Akka.Event;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution
{
    public class CommandExecutionPerfFixtureSql
    {
        private Counter _counter;
        private readonly ITestOutputHelper _testOutputHelper;
        private NodeTestFixture _fixture;

        public CommandExecutionPerfFixtureSql(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _fixture = new BalloonFixture()
                      {
                          Output = _testOutputHelper,
                          AkkaConfig = new StressTestAkkaConfiguration(LogLevel.WarningLevel),
                          LogLevel = LogEventLevel.Warning
                      }.UseSqlPersistence();

            _fixture.CreateNode().Wait();
            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(101,11);

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions without projections with sql server",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 100)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void MeasureCommandExecutionWithoutProjectionsUsingSql()
        {
            Scenario.Execute(_fixture.Node, p => _counter.Increment());
        }


        [PerfCleanup]
        public void Cleanup()
        {
          // var totalCommandsToIssue = Scenario.CommandPlans.Count();
          //
          // var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(_fixture.AkkaConfig.Persistence.SnapshotConnectionString).Options;
          // var rawJournalRepository = new RawJournalRepository(dbContextOptions);
          // var count = rawJournalRepository.TotalCount();
          // if (count != totalCommandsToIssue)
          // {
          //     _fixture.Output.WriteLine($"!!! Journal contains only {count} of {totalCommandsToIssue} !!!");
          //     Task.Delay(2000).Wait();
          //     count = rawJournalRepository.TotalCount();
          //     _fixture.Output.WriteLine($"After 2 sec Journal contains {count} of {totalCommandsToIssue}");
          // }

            _fixture.Dispose();
        }
    }
}