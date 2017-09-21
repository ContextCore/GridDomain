using System.Diagnostics;
using GridDomain.Node;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution
{

    public abstract class ScenarionPerfTest
    {
        private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        protected IGridDomainNode Node;

        protected ScenarionPerfTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public virtual void Setup(BenchmarkContext context)
        {
            OnSetup();
            Node = CreateNode();
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }
        protected virtual void OnSetup() { }

        protected abstract INodeScenario Scenario { get; }
        protected abstract IGridDomainNode CreateNode();

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions without projections in memory",
                       NumberOfIterations = 3, 
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 1)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void MeasureCommandsPerSecond()
        {
            Scenario.Execute(Node, p => _counter.Increment()).Wait();
        }

        [PerfCleanup]
        public virtual void Cleanup()
        {
            Node.Dispose();
        }
    }
}