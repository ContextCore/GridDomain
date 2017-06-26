using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestAggregateCommandHandler : AggregateCommandsHandler<TestAggregate>
    {
        public TestAggregateCommandHandler()
        {
            Map<SuccessCommand>((cmd, agg) => agg.Success(cmd.Text));
            Map<TimeoutCommand>((cmd, agg) => agg.LongTime(cmd.Text, cmd.Timeout));
            Map<FailCommand>((cmd, agg) => agg.Failure(cmd.Timeout));
        }
    }
}