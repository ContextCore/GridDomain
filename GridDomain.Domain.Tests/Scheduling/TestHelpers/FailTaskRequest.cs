using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public class FailTaskRequest : ScheduledRequest
    {
        public FailTaskRequest(string taskId) : base(taskId)
        {
        }
    }
}