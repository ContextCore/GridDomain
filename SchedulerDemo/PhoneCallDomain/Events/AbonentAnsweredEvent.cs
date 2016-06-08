using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.PhoneCallDomain.Events
{
    public class AbonentAnsweredEvent : DomainEvent
    {
        public Guid AbonentId => SourceId;

        public AbonentAnsweredEvent(Guid abonentId) : base(abonentId)
        {
        }
    }
}