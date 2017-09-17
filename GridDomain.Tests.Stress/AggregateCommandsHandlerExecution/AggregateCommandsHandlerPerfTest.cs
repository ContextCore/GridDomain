using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution
{
    public class AggregateCommandsHandlerPerfTest
    {

        private const string TotalCommandsExecutedCounter = nameof(TotalCommandsExecutedCounter);
        private Counter _counter;
        private ITestOutputHelper _testOutputHelper;
        private Balloon _aggregate;
        private BalloonCommandHandler _balloonCommandHandler;

        public AggregateCommandsHandlerPerfTest(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            _testOutputHelper = output;
            Trace.Listeners.Add(new XunitTraceListener(_testOutputHelper));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
            _aggregate = new Balloon(Guid.NewGuid(), "test balloon");
            _balloonCommandHandler = new BalloonCommandHandler();
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measure aggregate commands handler performance",
            NumberOfIterations = 5,
            RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 100000)]
        public void AggregateCommandsHandlerThroughput()
        {
            var writeTitleCommand = new WriteTitleCommand(123, _aggregate.Id);

            Task Persist(Aggregate a)
            {
                a.PersistAll();
                return Task.CompletedTask;
            }

            for(int i = 0; i < 110000; i++)
            {
                _balloonCommandHandler.ExecuteAsync(_aggregate,writeTitleCommand,Persist)
                                      .Wait();
                _counter.Increment();
            }
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measure aggregate commands handler performance",
            NumberOfIterations = 5,
            RunMode = RunMode.Iterations,
            RunTimeMilliseconds = 1000,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThan, 100000)]
        public void AggregateDirectThroughput()
        {
            for(int i = 0; i < 110000; i++)
            {
                _aggregate.WriteNewTitle(i);
                _aggregate.PersistAll();
                _counter.Increment();
            }
        }
    }
}
