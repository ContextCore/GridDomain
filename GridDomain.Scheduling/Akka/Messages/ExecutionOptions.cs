using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExecutionOptions
    {
        public Type SuccessEventType { get; }
        public DateTime RunAt { get; }
        public TimeSpan Timeout { get; }

        public ExecutionOptions(DateTime runAt, Type successEventType, TimeSpan? timeout = null)
        {
            RunAt = runAt;
            SuccessEventType = successEventType;
            Timeout = timeout ?? TimeSpan.FromMinutes(1);
        }
    }
}