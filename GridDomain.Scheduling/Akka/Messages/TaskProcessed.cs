namespace GridDomain.Scheduling.Akka.Messages
{
    public sealed class TaskProcessed
    {
        public string TaskId { get; private set; }
        public TaskProcessed(string taskId)
        {
            TaskId = taskId;
        }
    }
}