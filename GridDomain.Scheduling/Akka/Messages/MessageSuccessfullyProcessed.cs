namespace GridDomain.Scheduling.Akka.Messages
{
    public class MessageSuccessfullyProcessed : IMessageProcessingStatusChanged
    {
        public string TaskId { get;}
        public MessageSuccessfullyProcessed(string taskId)
        {
            TaskId = taskId;
        }
    }
}