using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestMessage : ScheduledMessage
    {
        public TestMessage(string taskId, string group) : base(taskId, group)
        {
        }
    }
}