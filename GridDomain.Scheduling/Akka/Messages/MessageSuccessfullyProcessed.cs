namespace GridDomain.Scheduling.Akka.Messages
{
    public class MessageSuccessfullyProcessed
    {
        public string TaskId { get; private set; }
        public MessageSuccessfullyProcessed(string taskId)
        {
            TaskId = taskId;
        }
    }
}