using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Schedule
    {
        public ScheduledCommand Command { get; }
        public DateTime RunAt { get; }
        public TimeSpan ExecutionTimeout { get; }

        public Schedule(ScheduledCommand command, DateTime runAt, TimeSpan executionTimeout)
        {
            Command = command;
            RunAt = runAt;
            ExecutionTimeout = executionTimeout;
        }
    }
}