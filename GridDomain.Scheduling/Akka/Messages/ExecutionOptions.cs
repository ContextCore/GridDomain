using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public abstract class ExecutionOptions
    {
        public abstract Type SuccessEventType { get; }
        public DateTime RunAt { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public ExecutionOptions(DateTime runAt, TimeSpan timeout = default(TimeSpan))
        {
            RunAt = runAt;
            Timeout = timeout == default(TimeSpan) ? TimeSpan.FromMinutes(1) : timeout;
        }
    }
    public class ExecutionOptions<TSuccessEvent> : ExecutionOptions where TSuccessEvent : DomainEvent
    {
        public override Type SuccessEventType => typeof(TSuccessEvent);

        public ExecutionOptions(DateTime runAt, TimeSpan timeout = new TimeSpan()) : base(runAt, timeout)
        {
        }
    }
}