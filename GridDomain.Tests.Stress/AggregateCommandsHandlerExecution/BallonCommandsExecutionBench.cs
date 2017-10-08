using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using NBench;
using Pro.NBench.xUnit.XunitExtensions;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public abstract class BallonCommandsExecutionBench<T> where T : Aggregate
    {
        private const string TotalCommandsExecutedCounter = nameof(TotalCommandsExecutedCounter);
        private Counter _counter;

        protected abstract IAggregateCommandsHandler<T> CommandsHandler { get; }

        private IEnumerable<ICommand> CreateCommandPlan(int aggregateNum, int changeNum)
        {
            var random = new Random();
            for (int i = 0; i < aggregateNum; i++)
            {
                var aggregateId = Guid.NewGuid();
                yield return new InflateNewBallonCommand(random.Next(),aggregateId);
                for (int j = 0; j < changeNum; j++)
                    yield return new WriteTitleCommand(random.Next(),aggregateId);
            }
        }

        private ICommand[] _commndsToExecute;
        private FakeEventStore _fakeEventStore;

        protected BallonCommandsExecutionBench(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _counter = context.GetCounter(TotalCommandsExecutedCounter);
            _commndsToExecute = CreateCommandPlan(1000, 1000).ToArray();
            _fakeEventStore = new FakeEventStore();
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measure aggregate commands handler performance",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Iterations,
                       TestMode = TestMode.Test)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        [CounterThroughputAssertion(TotalCommandsExecutedCounter, MustBe.GreaterThanOrEqualTo, 10000)]
        public void AggregateCommandsHandlerThroughput()
        {
            T aggregate = AggregateFactory.BuildEmpty<T>();

            
            aggregate.InitEventStore(_fakeEventStore);

            foreach (var cmd in _commndsToExecute)
            {
               

                aggregate = CommandsHandler.ExecuteAsync(aggregate,
                                                         cmd,
                                                         _fakeEventStore)
                                           .Result;
                _counter.Increment();
            }
        }
    }
}