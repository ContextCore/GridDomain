using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class TestRequest : ProcessScheduledTaskRequest
    {
        public TestRequest(string taskId = "test") : base(taskId)
        {
        }
    }
}