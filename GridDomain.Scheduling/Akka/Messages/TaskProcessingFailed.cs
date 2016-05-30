using System;
using Akka.Actor;

namespace GridDomain.Scheduling.Akka.Messages
{
    public sealed class TaskProcessingFailed : Failure, IProcessingResult
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