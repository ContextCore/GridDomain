using System;
using Akka.Actor;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class TaskProcessingFailed : Failure
    {
        public string TaskId { get; private set; }

        public TaskProcessingFailed(string taskId, Exception cause)
        {
            TaskId = taskId;
            Exception = cause;
            Timestamp = DateTime.UtcNow;
        }
    }
}