using System.Diagnostics;
using GridDomain.Node;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution
{

    public abstract class ScenarionInMemPerfTest
    {
        private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        private IGridDomainNode _gridDomainNode;

        protected ScenarionInMemPerfTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            OnSetup();
            _gridDomainNode = CreateNode();
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
        public void MeasureCommandExecutionWithoutProjectionsInMemory()
        {
            Scenario.Execute(_gridDomainNode, p => _counter.Increment());
        }

        [PerfCleanup]
        public void Cleanup()
        {
            _gridDomainNode.Dispose();
        }
    }
}