using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.PhoneCallDomain.Events
{
    public class AbonentReceivingIncomingCallEvent : DomainEvent
    {
        public Guid AbonentId => SourceId;

        public AbonentReceivingIncomingCallEvent(Guid abonentId) : base(abonentId)
        {
        }
    }
}