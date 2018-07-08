using System;

namespace GridDomain.Scheduling.Quartz
{
    public class ExecutionOptions
    {
        public ExecutionOptions(DateTime runAt,
                                TimeSpan? timeout = null) 
        {
            RunAt = runAt;
            Timeout = timeout ?? TimeSpan.FromMinutes(1);
        }

        public static ExecutionOptions ForCommand(DateTime runAt, TimeSpan? timeout = null)
        {
            return new ExecutionOptions(runAt, timeout);
        }

        public DateTime RunAt { get; }
        public TimeSpan Timeout { get; }
    }
}