using GridDomain.CQRS.Messaging.MessageRouting;
using SchedulerDemo.Events;
using SchedulerDemo.ScheduledCommands;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregateCommadHandler : AggregateCommandsHandler<ConsoleAggregate>
    {
        public ConsoleAggregateCommadHandler() : base(null)
        {
            Map<WriteToConsoleScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.WriteToConsole(c.Text);
            });

            Map<LongTimeScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.LongOperation(c.Text, c.Timeout);
            });

            Map<FailScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.FailOperation();
            });
        }
    }
}