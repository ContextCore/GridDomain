namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unscheduled
    {
        public string TaskId { get; }

        public Unscheduled(string taskId)
        {
            TaskId = taskId;
        }
    }
}