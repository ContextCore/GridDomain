using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class FailScheduledMessage : ScheduledMessage
    {
        public FailScheduledMessage(string taskId, string group) : base(taskId, group)
        {
        }
    }
}