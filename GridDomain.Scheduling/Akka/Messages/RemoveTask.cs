namespace GridDomain.Scheduling.Akka.Messages
{
    public class RemoveTask
    {
        public string TaskId { get; }

        public RemoveTask(string taskId)
        {
            TaskId = taskId;
        }
    }
}