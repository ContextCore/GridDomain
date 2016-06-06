using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailTaskMessage : ScheduledMessage
    {
        public FailTaskMessage(string taskId, string group) : base(taskId, group)
        {
        }
    }
}