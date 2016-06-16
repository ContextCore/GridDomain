using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class CompleteJob : Command
    {
        public string TaskId { get; private set; }
        public string Group { get; private set; }

        public CompleteJob(string taskId, string group)
        {
            TaskId = taskId;
            Group = group;
        }
    }
}