using System;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ExecutionOptions
    {
        public Type SuccessEventType { get; }
        public DateTime RunAt { get; private set; }
        public TimeSpan Timeout { get; private set; }

        public ExecutionOptions(DateTime runAt, Type succesEventType, TimeSpan timeout = default(TimeSpan))
        {
            RunAt = runAt;
            SuccessEventType = succesEventType;
            Timeout = timeout == default(TimeSpan) ? TimeSpan.FromMinutes(1) : timeout;
        }
    }
    public class ExecutionOptions<TSuccessEvent> : ExecutionOptions where TSuccessEvent : DomainEvent
    {
       // public new TSuccessEvent SuccessEventType => typeof(TSuccessEvent);

        public ExecutionOptions(DateTime runAt, TimeSpan timeout = new TimeSpan()) : base(runAt, typeof(TSuccessEvent),timeout)
        {
        }
    }
}