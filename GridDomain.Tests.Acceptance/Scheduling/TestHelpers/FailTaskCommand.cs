using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailTaskCommand : ScheduledCommand
    {
        public FailTaskCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}