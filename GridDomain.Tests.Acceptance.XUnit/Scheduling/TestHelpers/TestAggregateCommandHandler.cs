using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestAggregateCommandHandler : AggregateCommandsHandler<TestAggregate>
    {
        public TestAggregateCommandHandler()
        {
            Map<SuccessCommand>(cmd => cmd.Id, (cmd, agg) => agg.Success(cmd.Text));
            Map<TimeoutCommand>(cmd => cmd.Id, (cmd, agg) => agg.LongTime(cmd.Text, cmd.Timeout));
            Map<FailCommand>(cmd => cmd.Id, (cmd, agg) => agg.Failure(cmd.Timeout));
        }
    }
}