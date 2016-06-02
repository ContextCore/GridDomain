using System;

namespace GridDomain.Scheduling.Akka.Tasks
{
    public abstract class ScheduledRequest
    {
        public string TaskId { get; }

        protected ScheduledRequest(string taskId)
        {
            TaskId = taskId ?? Guid.NewGuid().ToString();
        }
    }
}