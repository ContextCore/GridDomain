using GridDomain.Scheduling.Quartz;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class AlreadyScheduled
    {
        public AlreadyScheduled(ScheduleKey key)
        {
            Key = key;
        }

        public ScheduleKey Key { get; }
    }
}