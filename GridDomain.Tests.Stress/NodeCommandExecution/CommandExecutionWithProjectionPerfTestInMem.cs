using System.Diagnostics;
using Akka.Event;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Unit;
using Microsoft.EntityFrameworkCore;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress {
    public class CommandExecutionWithProjectionPerfTestInMem
    {
        private Counter _counter;
        private readonly ITestOutputHelper _testOutputHelper;
        private NodeTestFixture _fixture;
        private DbContextOptions<BalloonContext> _dbContextOptions;

        public CommandExecutionWithProjectionPerfTestInMem(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _dbContextOptions = new DbContextOptionsBuilder<BalloonContext>().UseInMemoryDatabase(nameof(CommandExecutionWithProjectionPerfTestInMem)).Options;
            using (var ctx = new BalloonContext(_dbContextOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }

           _fixture = new BalloonWithProjectionFixture(_dbContextOptions)
           {
               Output = _testOutputHelper,
               AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
               LogLevel = LogEventLevel.Error
           };

            _fixture.CreateNode().Wait();

            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenarioProjection(10, 10);

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions with projections in sql",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        //MAX: 400, need several launches to warm up sql server
        // 50 as test is run on 'slow' appveyor
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 50)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void MeasureCommandExecutionWithProjectionsInMem()
        {
            Scenario.Execute(_fixture.Node, p => _counter.Increment());
        }

        [PerfCleanup]
        public void Cleanup()
        {
            _fixture.Dispose();
        }
    }
}