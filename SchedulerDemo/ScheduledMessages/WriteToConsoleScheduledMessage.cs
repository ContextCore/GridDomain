using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class WriteToConsoleScheduledMessage : ScheduledMessage
    {
        public WriteToConsoleScheduledMessage(string taskId, string group) : base(taskId, group)
        {
            
        }
    }
}