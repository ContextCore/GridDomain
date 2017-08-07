using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridGomain.Tests.Stress {
    public class NBenchAvailabilityTest
    {
        private const string TotalCommandsExecutedCounter = nameof(TotalCommandsExecutedCounter);
        private Counter _counter;
        private ITestOutputHelper _testOutputHelper;

        public NBenchAvailabilityTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            _testOutputHelper = output;
            Trace.Listeners.Add(new XunitTraceListener(_testOutputHelper));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }


        [NBenchFact]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 400)]

        public void SimpleFact()
        {
            _counter.Increment();
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var allTypes = executingAssembly.GetReferencedAssemblies()
                                                .Select(Assembly.Load)
                                                .Concat(new[] { executingAssembly })
                                                .SelectMany(a => a.GetTypes())
                                                .ToArray();
            }
            catch(Exception ex)
            {
               // foreach(var e in ex.LoaderExceptions)
                    _testOutputHelper.WriteLine(ex.ToString());
            }
            _testOutputHelper.WriteLine("ok");
        }
        [NBenchFact]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 5, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 400)]
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