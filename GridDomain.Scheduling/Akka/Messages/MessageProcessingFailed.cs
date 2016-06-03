using System;
using Akka.Actor;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class MessageProcessingFailed : Failure
    {
        public string TaskId { get; private set; }

        public MessageProcessingFailed(string taskId, Exception cause)
        {
            TaskId = taskId;
            Exception = cause;
            Timestamp = DateTime.UtcNow;
        }
    }
}