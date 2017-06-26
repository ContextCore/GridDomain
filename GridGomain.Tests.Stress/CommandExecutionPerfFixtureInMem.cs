using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit;
using GridDomain.Tools.Repositories.RawDataRepositories;
using Microsoft.Practices.Unity;
using Pro.NBench.xUnit.XunitExtensions;
using NBench;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridGomain.Tests.Stress
{
    public class CommandExecutionPerfFixtureInMem
    {
        private Counter _counter;
        private NodeTestFixture Fixture { get; }

        public CommandExecutionPerfFixtureInMem(ITestOutputHelper output)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new XunitTraceListener(output));

            Fixture = new BalloonFixture
                      {
                          Output = output,
                          InMemory = true,
                          AkkaConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel),
                          LogLevel = LogEventLevel.Error
                      };
        }

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            Fixture.CreateNode().Wait();
            _counter = context.GetCounter("TotalCommandsExecutedCounter");
        }

        private INodeScenario Scenario { get; } = new BalloonsCreationAndChangeScenario(101, 11);

        [NBenchFact]
        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
                       NumberOfIterations = 5, RunMode = RunMode.Iterations,
                       RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TotalCommandsExecutedCounter", MustBe.GreaterThan, 400)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        //MAX: 500
        public void Benchmark()
        {
            Scenario.Execute(Fixture.Node, p => _counter.Increment());
        }

        [PerfCleanup]
        public void Cleanup()
        {
            Fixture.Dispose();
        }
    }
}