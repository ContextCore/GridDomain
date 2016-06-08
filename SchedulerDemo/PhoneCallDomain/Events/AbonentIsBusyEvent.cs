using System;
using GridDomain.EventSourcing;

namespace SchedulerDemo.PhoneCallDomain.Events
{
    public class AbonentIsBusyEvent : DomainEvent
    {
        public Guid AbonentId => SourceId;

        public AbonentIsBusyEvent(Guid abonentId) : base(abonentId)
        {
        }
    }
}