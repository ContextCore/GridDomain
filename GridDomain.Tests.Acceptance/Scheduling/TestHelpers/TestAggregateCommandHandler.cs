using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestAggregateCommandHandler : AggregateCommandsHandler<TestAggregate>
    {
        public TestAggregateCommandHandler()
        {
            Map<SuccessCommand>(cmd => cmd.Id, (cmd, agg) => agg.Success(cmd.TaskId));
            Map<TimeoutCommand>(cmd => cmd.Id, (cmd, agg) => agg.LongTime(cmd.TaskId, cmd.Timeout));
            Map<FailCommand>(cmd => cmd.Id, (cmd, agg) => agg.Failure());
        }
    }
}