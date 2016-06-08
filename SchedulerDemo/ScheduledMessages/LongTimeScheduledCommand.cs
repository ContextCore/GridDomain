using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class LongTimeScheduledCommand : ScheduledCommand
    {
        public int SecondsToExecute { get; }

        public LongTimeScheduledCommand(string taskId, string group, int secondsToExecute) : base(taskId, @group)
        {
            SecondsToExecute = secondsToExecute;
        }
    }
}