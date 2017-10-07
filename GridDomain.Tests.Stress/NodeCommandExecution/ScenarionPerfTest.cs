using System.Diagnostics;
using GridDomain.Node;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution
{
    //it is performance test, not pure xunit
#pragma warning disable xUnit1013
    public abstract class ScenarionPerfTest
    {
        protected const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        protected IGridDomainNode Node;

        protected ScenarionPerfTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            OnSetup();
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }

        internal virtual void OnSetup()
        {
            Node = CreateNode();
        }

        protected abstract INodeScenario Scenario { get; }
        internal abstract IGridDomainNode CreateNode();

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring command executions without projections in memory",
                       NumberOfIterations = 3, 
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThanOrEqualTo, 0)]
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