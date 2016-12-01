using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestAggregateCommandHandler : AggregateCommandsHandler<TestAggregate>
    {
        public TestAggregateCommandHandler():base(null)
        {
            Map<SuccessCommand>(cmd => cmd.Id, (cmd, agg) => agg.Success(cmd.Text));
            Map<TimeoutCommand>(cmd => cmd.Id, (cmd, agg) => agg.LongTime(cmd.Text, cmd.Timeout));
            Map<FailCommand>(cmd => cmd.Id, (cmd, agg) => agg.Failure(cmd.Timeout));
            Map<PlanFailuresCommand>(cmd => cmd.AggregateId, (cmd, agg) => agg.PlanFailures(cmd.FailsNum));
            Map<FailIfPlannedCommand>(cmd => cmd.AggregateId, (cmd, agg) => agg.FailIfPlanned(cmd.Timeout));
            Map<FailCommand>(cmd => cmd.Id, (cmd, agg) => agg.Failure(cmd.Timeout));
        }
    }
}