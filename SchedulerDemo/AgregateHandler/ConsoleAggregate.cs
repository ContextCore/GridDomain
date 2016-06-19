using System;
using System.Threading;
using CommonDomain.Core;
using GridDomain.Scheduling.Akka.Messages;

namespace SchedulerDemo.AgregateHandler
{
    public class ConsoleAggregate : AggregateBase
    {
        private ConsoleAggregate(Guid id)
        {
            Id = id;
        }

        public void Apply(ScheduledCommandSuccessfullyProcessed @event)
        {
            
        }
        public void Apply(ScheduledCommandProcessingFailed @event)
        {
            
        }

        public void WriteToConsole(string text)
        {
            Console.WriteLine($"Handled {text}");
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }

        public void LongOperation(string taskId, TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            Console.WriteLine($"Long operation handled {taskId}");
            RaiseEvent(new ScheduledCommandSuccessfullyProcessed(Id));
        }


        public void FailOperation()
        {
            RaiseEvent(new ScheduledCommandProcessingFailed(Id,new InvalidOperationException("ohmagawd")));
        }
    }
}