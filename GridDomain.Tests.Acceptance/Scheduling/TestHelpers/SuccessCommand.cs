using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class SuccessCommand : ScheduledCommand
    {
        public SuccessCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}