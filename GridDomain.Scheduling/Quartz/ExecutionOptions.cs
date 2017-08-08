using System;

namespace GridDomain.Scheduling.Quartz
{
    public class ExecutionOptions
    {
        public ExecutionOptions(DateTime runAt,
                                Type succesEventType,
                                Guid successMessageId,
                                TimeSpan? timeout = null) 
        {
            RunAt = runAt;
            SuccesEventType = succesEventType;
            Timeout = timeout ?? TimeSpan.FromMinutes(1);
            SuccessMessageId = successMessageId;
        }

        public static ExecutionOptions ForCommand(DateTime runAt, Type succesEventType, TimeSpan? timeout = null)
        {
            return new ExecutionOptions(runAt, succesEventType, Guid.Empty, timeout);
        }

        public Type SuccesEventType { get; }
        public DateTime RunAt { get; }
        public TimeSpan Timeout { get; }
        public Guid SuccessMessageId { get; }
    }
}