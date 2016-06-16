using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class WriteToConsoleScheduledCommand : ScheduledCommand
    {
        public WriteToConsoleScheduledCommand(string taskId, string group) : base(taskId, group)
        {

        }
    }
}