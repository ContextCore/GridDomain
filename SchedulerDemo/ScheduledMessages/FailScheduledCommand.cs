using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class FailScheduledCommand : ScheduledCommand
    {
        public FailScheduledCommand(string taskId, string group) : base(taskId, group)
        {
        }
    }
}