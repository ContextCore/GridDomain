using System;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class AddTask
    {
        public ScheduledRequest Request { get; }
        public DateTime RunAt { get; }
        public TimeSpan ExecutionTimeout { get; }

        public AddTask(ScheduledRequest request, DateTime runAt, TimeSpan executionTimeout)
        {
            Request = request;
            RunAt = runAt;
            ExecutionTimeout = executionTimeout;
        }
    }
}