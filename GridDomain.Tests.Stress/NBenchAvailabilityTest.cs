using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress {
    //it is performance test, not pure xunit
#pragma warning disable xUnit1013
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


        [NBenchFact(Skip = "only for debugging purposes")]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 1, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 1)]

        public void Counter_and_assemblies_load()
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
        [NBenchFact(Skip = "only for debugging purposes")]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 5, RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 400)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void Counter_test()
        {
            _counter.Increment();
        }

        [PerfCleanup]
        public void Cleanup()
        {
        }
    }
}