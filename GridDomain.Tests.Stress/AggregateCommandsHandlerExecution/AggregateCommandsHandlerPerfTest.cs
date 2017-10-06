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
//it is performance test, not pure xunit
#pragma warning disable xUnit1013
    public class AggregateCommandsHandlerPerfTest
    {

        private const string TotalCommandsExecutedCounter = nameof(TotalCommandsExecutedCounter);
        private Counter _counter;
        private ITestOutputHelper _testOutputHelper;
        private Balloon _aggregate;
        private BalloonCommandHandler _balloonCommandHandler;
        private WriteTitleCommand _writeTitleCommand;

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
            _writeTitleCommand = new WriteTitleCommand(123, _aggregate.Id);
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measure aggregate commands handler performance",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Throughput,
                       RunTimeMilliseconds = 1000,
                       TestMode = TestMode.Measurement)]
        [CounterMeasurement(TotalCommandsExecutedCounter)]
        public void AggregateCommandsHandlerThroughput()
        {
            _balloonCommandHandler.ExecuteAsync(_aggregate,_writeTitleCommand,a =>
                                                                              {
                                                                                  a.PersistAll();
                                                                                  return Task.CompletedTask;
                                                                              }).Wait();
            _counter.Increment();
        }

        [NBenchFact]
        [PerfBenchmark(Description = "Measure aggregate commands handler performance",
                       NumberOfIterations = 3,
                       RunMode = RunMode.Throughput,
                       RunTimeMilliseconds = 1000,
                       TestMode = TestMode.Measurement)]
        [CounterMeasurement(TotalCommandsExecutedCounter)]
        public void AggregateDirectThroughput()
        {
             _aggregate.WriteNewTitle(234);
             _aggregate.PersistAll();
             _counter.Increment();
        }
    }
}
