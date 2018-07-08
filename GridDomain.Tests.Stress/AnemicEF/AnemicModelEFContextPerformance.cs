using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Tests.Acceptance.BalloonDomain;
using Microsoft.EntityFrameworkCore;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AnemicEF {
#pragma warning disable xUnit1013 //It is nbench test
    public abstract class AnemicModelEFContextPerformance
    {
        private readonly Func<BalloonContext> _contextCreator;
        private readonly IEFCommand[] _efCommands;
        private const string TotalCommandsExecutedCounter = "TotalCommandsExecutedCounter";
        private Counter _counter;
        private IEnumerable<InflateNewBallonCommandEF> _inflateNewBallonCommandEfs;

        protected AnemicModelEFContextPerformance(ITestOutputHelper output, DbContextOptions<BalloonContext> options) 
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
            _contextCreator = () => new BalloonContext(options);
            _efCommands = Enumerable.Range(0, 20)
                                    .SelectMany(n => CreateAggregatePlan(20))
                                    .ToArray();
            _inflateNewBallonCommandEfs = Enumerable.Range(0, 100)
                                                    .Select(i => new InflateNewBallonCommandEF(Guid.NewGuid()
                                                                                                   .ToString(),
                                                                                               "1",
                                                                                               () => new BalloonContext()));
        }
        [PerfSetup]

        public virtual void Setup(BenchmarkContext context)

        {
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measuring executions equivalent to commands by anemic model",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 50)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]

        public void MeasureCommandExecution()
        {
            Task.WhenAll(_efCommands.Select(c => c.Execute()
                                                  .ContinueWith(t => _counter.Increment())))
                .Wait();
        }
        
        [NBenchFact]
        [PerfBenchmark(Description = "Measuring row inserts into a tabla",
            NumberOfIterations = 3,
            RunMode = RunMode.Iterations,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 50)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]

        public void MeasureRowInsertCommandExecution()
        {
            Task.WhenAll(_inflateNewBallonCommandEfs.Select(c => c.Execute()
                                                  .ContinueWith(t => _counter.Increment())))
                .Wait();
        }

        [PerfCleanup]
        public void Cleanup()
        {
        }

        private IEnumerable<IEFCommand> CreateAggregatePlan(int changeAmount)
        {
            var random = new Random();
            var newGuid = Guid.NewGuid().ToString();
            yield return new InflateNewBallonCommandEF(newGuid,random.Next().ToString(),_contextCreator);
            for(var num = 0; num < changeAmount; num++)
                yield return new UpdateBallonCommandEF(newGuid,random.Next().ToString(), _contextCreator);
        }

    }
}