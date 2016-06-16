using GridDomain.CQRS.Messaging.MessageRouting;
using SchedulerDemo.Events;
using SchedulerDemo.ScheduledCommands;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregateCommadHandler : AggregateCommandsHandler<ConsoleAggregate>
    {
        public ConsoleAggregateCommadHandler()
        {
            Map<WriteToConsoleScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.WriteToConsole(c.TaskId, c.Group);
            });

            Map<LongTimeScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.LongOperation(c.TaskId, c.Group, c.Timeout);
            });

            Map<FailScheduledCommand>(c => c.Id, (c, a) =>
            {
                a.FailOperation(c.TaskId, c.Group);
            });
        }
    }
}