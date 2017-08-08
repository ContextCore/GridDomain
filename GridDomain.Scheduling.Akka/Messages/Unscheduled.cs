using GridDomain.Scheduling.Quartz;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unscheduled
    {
        public Unscheduled(ScheduleKey key)
        {
            Key = key;
        }

        public ScheduleKey Key { get; }
    }
}