using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestCommand : ScheduledCommand
    {
        public TestCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}