using System;

namespace GridDomain.Scheduling.Akka.Tasks
{
    public class AkkaScheduledTask
    {
        public DateTime RunAt { get; }
        public ProcessScheduledTaskRequest Request { get; }
        public virtual TimeSpan Timeout => TimeSpan.FromMinutes(1);

        public AkkaScheduledTask(DateTime runAt, ProcessScheduledTaskRequest request)
        {
            RunAt = runAt;
            Request = request;
        }
    }
}