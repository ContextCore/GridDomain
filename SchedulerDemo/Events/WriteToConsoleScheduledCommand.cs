using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.Events
{
    public class WriteToConsoleScheduledCommand : ScheduledCommand
    {
        public WriteToConsoleScheduledCommand(string taskId, string group) : base(taskId, group)
        {

        }
    }
}