using GridDomain.Scheduling.Akka.Tasks;

namespace SchedulerDemo.ScheduledMessages
{
    public class LongTimeScheduledMessage : ScheduledMessage
    {
        public int SecondsToExecute { get; }

        public LongTimeScheduledMessage(string taskId, string group, int secondsToExecute) : base(taskId, @group)
        {
            SecondsToExecute = secondsToExecute;
        }
    }
}