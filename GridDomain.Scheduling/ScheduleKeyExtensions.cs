using GridDomain.Scheduling.Quartz;
using Quartz;

namespace GridDomain.Scheduling
{
    public static class ScheduleKeyExtensions
    {
        public static JobKey ToJobKey(this ScheduleKey key)
        {
            return new JobKey(key.Name, key.Group);
        }
    }
}



