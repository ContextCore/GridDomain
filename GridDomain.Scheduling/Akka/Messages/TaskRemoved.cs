namespace GridDomain.Scheduling.Akka.Messages
{
    public class TaskRemoved
    {
        public string TaskId { get; }

        public TaskRemoved(string taskId)
        {
            TaskId = taskId;
        }
    }
}