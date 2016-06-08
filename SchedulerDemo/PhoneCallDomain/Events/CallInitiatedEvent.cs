using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.PhoneCallDomain.Events
{
    public class CallInitiatedEvent : DomainEvent
    {
        public Guid CallId => SourceId;
        public Guid Initiator { get; }
        public Guid Abonent { get; }

        public CallInitiatedEvent(Guid callId, Guid initiator, Guid abonent) : base(callId)
        {
            Initiator = initiator;
            Abonent = abonent;
        }
    }
}