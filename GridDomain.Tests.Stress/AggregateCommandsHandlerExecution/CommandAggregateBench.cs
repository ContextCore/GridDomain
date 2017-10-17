using GridDomain.EventSourcing;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class CommandAggregateBench : BallonCommandsExecutionBench<BenchmarkBallonCommandAggregate>
    {
        public CommandAggregateBench(ITestOutputHelper output) : base(output) { }
        protected override IAggregateCommandsHandler<BenchmarkBallonCommandAggregate> CommandsHandler { get; }
            = CommandAggregateHandler.New<BenchmarkBallonCommandAggregate>();
    }
}