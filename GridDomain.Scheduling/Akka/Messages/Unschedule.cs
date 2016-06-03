namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unschedule
    {
        public string TaskId { get; }
        public string Group { get;  }

        public Unschedule(string taskId, string group)
        {
            TaskId = taskId;
            Group = @group;
        }
    }
}