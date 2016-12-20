namespace GridDomain.Scheduling.Akka.Messages
{
    public class Unschedule
    {
        public ScheduleKey Key { get; private set; }
        
        public Unschedule(ScheduleKey key)
        {
            Key = key;
        }
    }
}