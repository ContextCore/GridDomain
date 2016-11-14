using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExecutionOptions
    {
        public Type SuccesEventType { get; }
        public DateTime RunAt { get; }
        public TimeSpan Timeout { get; }

        public ExecutionOptions(DateTime runAt, Type succesEventType, TimeSpan? timeout = null)
        {
            RunAt = runAt;
            SuccesEventType = succesEventType;
            Timeout = timeout ?? TimeSpan.FromMinutes(1);
        }
    }

    public class ExecutionOptions<TSuccessEvent> : ExecutionOptions where TSuccessEvent : DomainEvent
    {
        public ExecutionOptions(DateTime runAt, TimeSpan? timeout = null) : base(runAt, typeof(TSuccessEvent), timeout)
        {
        }
    }
}