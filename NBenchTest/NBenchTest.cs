using System.Diagnostics;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace NBenchTest {
    public class NBenchTest
    {
        private Counter _counter;

        public NBenchTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 500, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 400)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void Benchmark()
        {
            _counter.Increment();
        }

        [PerfCleanup]
        public void Cleanup()
        {
        }
    }
}