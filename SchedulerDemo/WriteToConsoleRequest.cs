using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo
{
    public class WriteToConsoleRequest : ScheduledRequest
    {
        public WriteToConsoleRequest(string taskId) : base(taskId)
        {
            
        }
    }
}