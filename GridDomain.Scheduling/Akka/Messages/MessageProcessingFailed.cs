using System;
using Akka.Actor;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class MessageProcessingFailed : Failure, IMessageProcessingStatusChanged
    {
        public string TaskId { get; }

        public MessageProcessingFailed(string taskId, Exception cause)
        {
            TaskId = taskId;
            Exception = cause;
            Timestamp = DateTime.UtcNow;
        }
    }
}