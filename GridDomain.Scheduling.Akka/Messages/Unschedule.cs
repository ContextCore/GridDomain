using GridDomain.Scheduling.Quartz;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unschedule
    {
        public Unschedule(ScheduleKey key)
        {
            Key = key;
        }

        public ScheduleKey Key { get; private set; }
    }
}