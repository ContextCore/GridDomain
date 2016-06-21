namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unscheduled
    {
        public ScheduleKey Key { get; }

        public Unscheduled(ScheduleKey key)
        {
            Key = key;
        }
    }
}