using System.Diagnostics;
using Akka.Event;
using GridDomain.Tests.Unit;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class AggregateActorPerfFixtureInMem
    {
        private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        private readonly ITestOutputHelper _testOutputHelper;
        private NodeTestFixture _fixture;

        public AggregateActorPerfFixtureInMem(ITestOutputHelper output)
        {
            _testOutputHelper = output;
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _fixture = new BalloonFixture
                       {
                           Output = _testOutputHelper,
                           AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                           LogLevel = LogEventLevel.Error
                       };

            _fixture.CreateNode().Wait();
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(101, 11);

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions without projections in memory",
            NumberOfIterations = 3, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 100)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void MeasureCommandExecutionWithoutProjectionsInMemory()
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