namespace GridDomain.Scheduling.Akka.Messages
{
    public class AlreadyScheduled
    {
        public string TaskId { get; }
        public string Group { get;  }

        public AlreadyScheduled(string taskId, string group)
        {
            TaskId = taskId;
            Group = @group;
        }
    }
}