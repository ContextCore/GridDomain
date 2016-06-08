using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestCommand : ScheduledCommand
    {
        public TestCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}