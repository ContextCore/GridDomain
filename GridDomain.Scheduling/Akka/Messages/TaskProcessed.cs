namespace GridDomain.Scheduling.Akka.Messages
{
    public class TaskProcessed
    {
        public string TaskId { get; private set; }
        public TaskProcessed(string taskId)
        {
            TaskId = taskId;
        }
    }
}