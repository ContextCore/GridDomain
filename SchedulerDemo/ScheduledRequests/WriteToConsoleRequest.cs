using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledRequests
{
    public class WriteToConsoleRequest : ScheduledRequest
    {
        public WriteToConsoleRequest(string taskId) : base(taskId)
        {
            
        }
    }
}