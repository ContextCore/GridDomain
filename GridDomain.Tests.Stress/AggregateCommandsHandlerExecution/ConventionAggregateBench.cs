using GridDomain.EventSourcing;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class ConventionAggregateBench : BallonCommandsExecutionBench<BenchmarkBallonConventionAggregate>
    {
        public ConventionAggregateBench(ITestOutputHelper output) : base(output) { }
        protected override IAggregateCommandsHandler<BenchmarkBallonConventionAggregate> CommandsHandler { get; }
            = CommandAggregateHandler.New<BenchmarkBallonConventionAggregate>();
    }
}