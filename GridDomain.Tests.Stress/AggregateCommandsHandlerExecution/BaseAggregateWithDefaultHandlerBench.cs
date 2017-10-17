using GridDomain.EventSourcing;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class BaseAggregateWithDefaultHandlerBench : BallonCommandsExecutionBench<BenchmarkBalloonAggregate>
    {
        public BaseAggregateWithDefaultHandlerBench(ITestOutputHelper output) : base(output) { }
        protected override IAggregateCommandsHandler<BenchmarkBalloonAggregate> CommandsHandler { get; }
            = new BenchmarkBalloonAggregateCommandsHandler();
    }
}