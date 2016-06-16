using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class FailCommand : ScheduledCommand
    {
        public FailCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}