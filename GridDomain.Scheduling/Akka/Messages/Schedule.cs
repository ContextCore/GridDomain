using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Schedule
    {
        public ScheduledMessage Message { get; }
        public DateTime RunAt { get; }
        public TimeSpan ExecutionTimeout { get; }

        public Schedule(ScheduledMessage message, DateTime runAt, TimeSpan executionTimeout)
        {
            Message = message;
            RunAt = runAt;
            ExecutionTimeout = executionTimeout;
        }
    }
}