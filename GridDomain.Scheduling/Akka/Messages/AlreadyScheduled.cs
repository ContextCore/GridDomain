namespace GridDomain.Scheduling.Akka.Messages
{
    public class AlreadyScheduled
    {
        public ScheduleKey Key { get; }

        public AlreadyScheduled(ScheduleKey key)
        {
            Key = key;
        }
    }
}