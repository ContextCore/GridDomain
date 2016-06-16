using System;
using CommonDomain.Core;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregate : AggregateBase
    {
        private ConsoleAggregate(Guid id)
        {
            Id = id;
        }

        public void WriteToConsole(string taskId, string @group)
        {
            Console.WriteLine($"Handled {taskId} {group}");
            RaiseEvent(new WrittenToConsoleEvent(Id));
        }

        public void LongOperation(string taskId, string @group)
        {
            Console.WriteLine($"Long operation handled {taskId} {group}");
            RaiseEvent(new WrittenToConsoleEvent(Id));
        }


        public void FailOperation(string taskId, string @group)
        {
            throw new InvalidOperationException("ohmagawd");
            RaiseEvent(new FailScheduledCommand());
        }
    }
}