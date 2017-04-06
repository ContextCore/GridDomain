using GridDomain.Scheduling.Akka.Messages;
using Quartz;

namespace GridDomain.Scheduling.Integration
{
    public static class ScheduleKeyExtensions
    {
        public static JobKey ToJobKey(this ScheduleKey key)
        {
            return new JobKey(key.Name, key.Group);
        }
    }
}